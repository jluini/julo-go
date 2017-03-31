
using System.Collections.Generic;

namespace JuloGo {
	
	public class GoAnalyzerImpl : GoAnalyzer {
		GoGame game;
		Dictionary<BoardValue, TeamScore> teamScores;
		
		HashSet<BoardPosition> deadPositions;
		HashSet<BoardPosition> blackPositions;
		HashSet<BoardPosition> whitePositions;
		
		public GoAnalyzerImpl() {
			teamScores = new Dictionary<BoardValue, TeamScore>();
			teamScores[BoardValue.Black] = new TeamScore();
			teamScores[BoardValue.White] = new TeamScore();
			
			deadPositions = new HashSet<BoardPosition>();
			
			deadPositions = new HashSet<BoardPosition>();
			blackPositions = new HashSet<BoardPosition>();
			whitePositions = new HashSet<BoardPosition>();
		}
		
		public void setGame(GoGame game) {
			this.game = game;
			clear();
		}
		
		void clear() {
			teamScores[BoardValue.Black].stones    = 0;
			teamScores[BoardValue.Black].territory = 0;
			teamScores[BoardValue.White].stones    = 0;
			teamScores[BoardValue.White].territory = 0;
			
			deadPositions.Clear();
			blackPositions.Clear();
			whitePositions.Clear();
		}
		
		public Dictionary<BoardValue, TeamScore> getTeamScores() {
			return teamScores;
		}
		
		public void analyze() {
			clear();
			
			Dictionary<BoardPosition, BoardGroup> positionToGroup = new Dictionary<BoardPosition, BoardGroup>();
			
			foreach(BoardPosition pos in game) {
				UnityEngine.Assertions.Assert.IsNotNull(pos);
				if(positionToGroup.ContainsKey(pos)) {
					// next
					
				} else {
					BoardValue value = game.getCellValue(pos);
					BoardGroup group = new BoardGroup(value, pos);
					
					positionToGroup[pos] = group;
					
					expand(group, positionToGroup);
					
					if(group.color == BoardValue.Empty) {
						bool toBlack = group.isAdjacentTo(BoardValue.Black);
						bool toWhite = group.isAdjacentTo(BoardValue.White);
						if(toBlack && !toWhite) {
							teamScores[BoardValue.Black].territory += group.size;
							blackPositions.UnionWith(group.allPositions);
						} else if(toWhite && !toBlack) {
							teamScores[BoardValue.White].territory += group.size;
							whitePositions.UnionWith(group.allPositions);
						} else {
							deadPositions.UnionWith(group.allPositions);
						}
					} else {
						teamScores[group.color].stones += group.size;
						(group.color == BoardValue.Black ? blackPositions : whitePositions)
								.UnionWith(group.allPositions);
					}
				}
			}
		}
		public int getBlackPoints() {
			return teamScores[BoardValue.Black].getArea();
		}
		
		public int getWhitePoints() {
			return teamScores[BoardValue.White].getArea();
		}
		
		public IEnumerable<BoardPosition> getDeadPositions() {
			return deadPositions;
		}
		public IEnumerable<BoardPosition> getBlackPositions() {
			return blackPositions;
		}
		public IEnumerable<BoardPosition> getWhitePositions() {
			return whitePositions;
		}
		
		void expand(BoardGroup group, Dictionary<BoardPosition, BoardGroup> positionToGroup) {
			List<BoardPosition> ns = neighbors(group.currentPosition);
			
			foreach(BoardPosition n in ns) {
				BoardValue val = game.getCellValue(n);
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
		
		List<BoardPosition> neighbors(BoardPosition pos) {
			List<BoardPosition> ret = new List<BoardPosition>();
			BoardPosition p;
			p = game.getPosition(pos.row - 1, pos.col    );
			if(p != null)
				ret.Add(p);
			p = game.getPosition(pos.row + 1, pos.col    );
			if(p != null)
				ret.Add(p);
			p = game.getPosition(pos.row    , pos.col - 1);
			if(p != null)
				ret.Add(p);
			p = game.getPosition(pos.row    , pos.col + 1);
			if(p != null)
				ret.Add(p);
			
			return ret;
		}
	}
	
	class BoardGroup {
		public BoardValue color;
		public BoardPosition currentPosition;
		public HashSet<BoardPosition> allPositions;
		public int size;
		public HashSet<BoardValue> adjacentColors;
		
		public BoardGroup(BoardValue color, BoardPosition position) {
			this.color = color;
			this.currentPosition = position;
			this.allPositions = new HashSet<BoardPosition>();
			this.allPositions.Add(position);
			this.size = 1;
			this.adjacentColors = new HashSet<BoardValue>();
		}
		
		public void addPosition(BoardPosition pos) {
			currentPosition = pos;
			allPositions.Add(pos);
			size++;
		}
		
		public void addAdjacentColor(BoardValue adjColor) {
			UnityEngine.Assertions.Assert.AreNotEqual(adjColor, color);
			adjacentColors.Add(adjColor);
		}
		
		public bool isAdjacentTo(BoardValue adjColor) {
			UnityEngine.Assertions.Assert.AreNotEqual(adjColor, color);
			return adjacentColors.Contains(adjColor);
		}
		public override string ToString() {
			return color + " (" + size + ") at " + currentPosition;
		}
	}
	

}