
using System;
using System.Collections;
using System.Collections.Generic;
﻿
using UnityEngine;
using UnityEngine.Assertions;

namespace JuloGo {
	
	class BoardCell {
		public BoardPosition pos;
		public BoardValue value;
		
		public BoardCell() : this(new BoardPosition(), BoardValue.Empty) { }
		public BoardCell(BoardPosition pos, BoardValue value = BoardValue.Empty) {
			this.pos = pos;
			this.value = value;
		}
	}
	
	public class GoGameImpl : GoGame, GoGameListener {
		Dictionary<BoardValue, TeamScore> teamScores;
		Dictionary<BoardValue, TeamData> teamData;
		
		int numRows;
		int numCols;
		BoardCell[,] board;
		List<GoGameListener> listeners;
		
		BoardValue turn;
		
		List<GoMove> moves;
		int currentMoveNumber;
		
		public GoGameImpl(int rows, int cols) {
			turn = BoardValue.Black;
			
			initBoard(rows, cols);
			
			listeners = new List<GoGameListener>();
			
			teamScores = new Dictionary<BoardValue, TeamScore>();
			teamScores[BoardValue.Black] = new TeamScore();
			teamScores[BoardValue.White] = new TeamScore();
						
			teamData = new Dictionary<BoardValue, TeamData>();
			teamData[BoardValue.Black] = new TeamData();
			teamData[BoardValue.White] = new TeamData();
			
			moves = new List<GoMove>();
			currentMoveNumber = -1;
		}
		
		public void init() {
			updateData();
		}
		
		public void addListener(GoGameListener listener) {
			listeners.Add(listener);
		}
		
		public void passTurn() {
			GoMove newMove = new GoMove(turn, null);
			addMove(newMove);
			turn = otherPlayer(turn);
			updateData();
		}
		
		public bool playAt(BoardPosition pos) {
			bool ret = false;
			BoardCell cell = getCell(pos);
			
			int r = pos.row;
			int c = pos.col;
			
			
			if(cell.value != BoardValue.Empty) {
				return ret;
			}
			
			// temporarily marks cell as occupied
			cell.value = turn;
			
			HashSet<BoardCell> captured = new HashSet<BoardCell>();
			
			captured.UnionWith(capture(r - 1, c    , turn));
			captured.UnionWith(capture(r + 1, c    , turn));
			captured.UnionWith(capture(r    , c - 1, turn));
			captured.UnionWith(capture(r    , c + 1, turn));
			
			int numCaptured = captured.Count;
			if(numCaptured > 0) {
				cell.value = BoardValue.Empty;
				
				if(numCaptured > 1 || currentMoveNumber < 0 || currentMove().prisoners.Count != 1 || !captured.Contains(getCell(currentMove().pos))) {
					GoMove move = new GoMove(turn, pos);
					foreach(BoardCell cc in captured) {
						move.addPrisoner(cc.pos);
					}
					
					addMove(move);
					ret = true;
				}
			} else {
				HashSet<BoardCell> set = new HashSet<BoardCell>();
				bool isSuicide = !groupHasAnyLiberty(r, c, turn, set);
				
				cell.value = BoardValue.Empty;
				
				if(!isSuicide) {
					addMove(new GoMove(turn, pos));
					ret = true;
				}
			}
			
			if(ret) {
				turn = otherPlayer(turn);
				updateData();
			}
			
			return ret;
		}
		
		public BoardPosition getPosition(int row, int col) {
			if(isInBoard(row, col)) {
				return board[row, col].pos;
			} else {
				return null;
			}
		}
		
		public BoardValue getCellValue(BoardPosition pos) {
			return getCell(pos).value;
		}
		
		public bool undo() {
			if(currentMoveNumber < 0) {
				return false;
			}
			
			GoMove move = moves[currentMoveNumber];
			unapplyMove(move);
			currentMoveNumber--;
			
			turn = otherPlayer(turn);
			
			updateData();
			
			return true;
		}
		
		public bool redo() {
			if(currentMoveNumber == moves.Count - 1) {
				return false;
			}
			currentMoveNumber++;
			GoMove move = moves[currentMoveNumber];
			applyMove(move);
			
			turn = otherPlayer(turn);
			
			updateData();
			return true;
		}
		
		public void reset() {
			while(currentMoveNumber >= 0)
				undo();
		}
		
		public void goToEnd() {
			while(currentMoveNumber < moves.Count - 1)
				redo();
		}
		
		public void advance(int num) {
			if(num > 0) {
				int i = 0;
				while(currentMoveNumber < moves.Count - 1 && i < num) {
					redo();
					i++;
				}
			} else if(num < 0) {
				int i = 0;
				while(currentMoveNumber >= 0 && i < -num) {
					undo();
					i++;
				}
			}
		}
	
		public void setBoardPoint(BoardPosition pos, BoardValue value) {
			BoardCell cell = getCell(pos);
			cell.value = value;
			foreach(GoGameListener listener in listeners) {
				listener.setBoardPoint(pos, value);
			}
		}
		
		public void setData(BoardValue turn, int currentMoveNumber, int totalMoves, TeamData blackData, TeamData whiteData) {
			foreach(GoGameListener listener in listeners) {
				listener.setData(turn, currentMoveNumber, totalMoves, blackData, whiteData);
			}
		}
		
		public GoMove currentMove() {
			Assert.IsTrue(moves.Count > currentMoveNumber);
			if(currentMoveNumber < 0) {
				return null;
			} else {
				return moves[currentMoveNumber];
			}
		}
		
		class BoardGroup {
			public BoardValue color;
			public BoardPosition currentPosition;
			public int size;
			public HashSet<BoardValue> adjacentColors;
			
			public BoardGroup(BoardValue color, BoardPosition position) {
				this.color = color;
				this.currentPosition = position;
				this.size = 1;
				this.adjacentColors = new HashSet<BoardValue>();
			}
			
			public void addPosition(BoardPosition pos) {
				currentPosition = pos;
				size++;
			}
			
			public void addAdjacentColor(BoardValue adjColor) {
				Assert.AreNotEqual(adjColor, color);
				adjacentColors.Add(adjColor);
			}
			
			public bool isAdjacentTo(BoardValue adjColor) {
				Assert.AreNotEqual(adjColor, color);
				return adjacentColors.Contains(adjColor);
			}
			public override string ToString() {
				return color + " (" + size + ") at " + currentPosition;
			}
		}
		
		public Dictionary<BoardValue, TeamScore> getTeamScores() {
			return teamScores;
		}
		
		void calculateScores() {
			teamScores[BoardValue.Black].stones    = 0;
			teamScores[BoardValue.Black].territory = 0;
			teamScores[BoardValue.White].stones    = 0;
			teamScores[BoardValue.White].territory = 0;
			
			Dictionary<BoardPosition, BoardGroup> positionToGroup = new Dictionary<BoardPosition, BoardGroup>();
			
			int r = 0, c = 0;
			
			while(true) {
				BoardPosition pos = getPosition(r, c);
				Assert.IsNotNull(pos);
				if(positionToGroup.ContainsKey(pos)) {
					// next
					c++;
					if(c >= numCols) {
						r++;
						c = 0;
						if(r >= numRows) {
							break;
						}
					}
				} else {
					BoardCell cell = getCell(pos);
					BoardGroup group = new BoardGroup(cell.value, pos);
					
					positionToGroup[pos] = group;
					
					expand(group, positionToGroup);
					
					if(group.color == BoardValue.Empty) {
						bool toBlack = group.isAdjacentTo(BoardValue.Black);
						bool toWhite = group.isAdjacentTo(BoardValue.White);
						if(toBlack && !toWhite) {
							teamScores[BoardValue.Black].territory += group.size;
						} else if(toWhite && !toBlack) {
							teamScores[BoardValue.White].territory += group.size;
						}
					} else {
						teamScores[group.color].stones += group.size;
					}
				}
			}
		}
		
		void updateData() {
			calculateScores();
			setData(turn, currentMoveNumber + 1, moves.Count, teamData[BoardValue.Black], teamData[BoardValue.White]);
		}
		
		List<BoardPosition> neighbors(BoardPosition pos) {
			List<BoardPosition> ret = new List<BoardPosition>();
			BoardPosition p;
			p = getPosition(pos.row - 1, pos.col    );
			if(p != null)
				ret.Add(p);
			p = getPosition(pos.row + 1, pos.col    );
			if(p != null)
				ret.Add(p);
			p = getPosition(pos.row    , pos.col - 1);
			if(p != null)
				ret.Add(p);
			p = getPosition(pos.row    , pos.col + 1);
			if(p != null)
				ret.Add(p);
			
			return ret;
		}
		
		void expand(BoardGroup group, Dictionary<BoardPosition, BoardGroup> positionToGroup) {
			List<BoardPosition> ns = neighbors(group.currentPosition);
			
			foreach(BoardPosition n in ns) {
				BoardCell neighborCell = getCell(n);
				BoardValue val = neighborCell.value;
				if(val == group.color) {
					if(!positionToGroup.ContainsKey(n)) {
						group.addPosition(n);
						positionToGroup[n] = group;
						expand(group, positionToGroup);
					}
				} else {
					group.addAdjacentColor(val);
				}
			}
		}
		
		BoardCell getCell(BoardPosition pos) {
			return board[pos.row, pos.col];
		}
		
		bool isInBoard(int r, int c) {
			return r >= 0 && r < numRows && c >= 0 && c < numCols;
		}
		
		void addMove(GoMove move) {
			Assert.AreEqual(move.turn, this.turn);
			if(currentMoveNumber < moves.Count - 1) {
				for(int i = moves.Count - 1; i > currentMoveNumber; i--) {
					moves.RemoveAt(i);
					Debug.Log("Removing future at " + i);
				}
			}
			moves.Add(move);
			currentMoveNumber++;
			Assert.AreEqual(currentMoveNumber, moves.Count - 1);
			
			applyMove(move);
		}
		
		void applyMove(GoMove move) {
			Assert.AreEqual(move.turn, this.turn);
			
			if(move.pos == null) {
				teamData[move.turn].passes++;
				return;
			}
			
			BoardPosition pos = move.pos;
			BoardCell cell = board[pos.row, pos.col];
			
			Assert.AreEqual(cell.value, BoardValue.Empty);
			setBoardPoint(pos, move.turn);
			
			BoardValue other = otherPlayer(move.turn);
			
			foreach(BoardPosition p in move.prisoners) {
				BoardCell cc = board[p.row, p.col];
				Assert.AreEqual(cc.value, other);
				setBoardPoint(p, BoardValue.Empty);
				teamData[move.turn].prisoners++;
			}
		}
		
		void unapplyMove(GoMove move) {
			if(move.pos == null) {
				teamData[move.turn].passes--;
				return;
			}
			
			BoardCell cell = board[move.pos.row, move.pos.col];
			Assert.AreEqual(move.turn, cell.value);
			setBoardPoint(move.pos, BoardValue.Empty);
			
			BoardValue other = otherPlayer(move.turn);
			
			foreach(BoardPosition p in move.prisoners) {
				BoardCell cc = board[p.row, p.col];
				
				Assert.AreEqual(BoardValue.Empty, cc.value);
				setBoardPoint(p, other);
				teamData[move.turn].prisoners--;
			}
		}
		
		BoardValue otherPlayer(BoardValue player) {
			Assert.AreNotEqual(player, BoardValue.Empty);
			if(player == BoardValue.White)
				return BoardValue.Black;
			else
				return BoardValue.White;
		}
		
		HashSet<BoardCell> capture(int r, int c, BoardValue turn) {
			if(!isInBoard(r, c)) {
				return new HashSet<BoardCell>();
			}
			
			BoardValue value = board[r,c].value;
			
			if(value == BoardValue.Empty || value == turn) {
				return new HashSet<BoardCell>();
			}
			
			HashSet<BoardCell> set = new HashSet<BoardCell>();
			bool hasLiberties = groupHasAnyLiberty(r, c, otherPlayer(turn), set);
			
			if(!hasLiberties) {
				return set;
			}
			
			return new HashSet<BoardCell>();
		}
		bool groupHasAnyLiberty(int r, int c, BoardValue color, HashSet<BoardCell> currentGroup) {
			if(!isInBoard(r, c)) {
				return false;
			}
			BoardCell position = board[r, c];
			if(currentGroup.Contains(position)) {
				return false;
			}
			BoardValue value = position.value;
			
			if(value == BoardValue.Empty) {
				return true;
			} else {
				bool sameColor = (color == value);
				if(sameColor) {
					currentGroup.Add(position);
					
					bool ret = groupHasAnyLiberty(r - 1, c    , color, currentGroup);
					if(!ret)
						ret = groupHasAnyLiberty(r + 1, c    , color, currentGroup);
					if(!ret)
						ret = groupHasAnyLiberty(r    , c - 1, color, currentGroup);
					if(!ret)
						ret = groupHasAnyLiberty(r    , c + 1, color, currentGroup);
					
					return ret;
				} else {
					return false;
				}
			}
		}
		
		void initBoard(int rows, int cols) {
			this.numRows = rows;
			this.numCols = cols;
			
			board = new BoardCell[numRows, numCols];
			
			for(int r = 0; r < numRows; r++) {
				for(int c = 0; c < numCols; c++) {
					board[r, c] = new BoardCell(new BoardPosition(r, c), BoardValue.Empty);
				}
			}
		}
	}

}