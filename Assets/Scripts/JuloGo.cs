
using System.Collections.Generic;

namespace JuloGo {
	
	public interface GoGame {
		void init();
		void addListener(GoGameListener listener);
		
		void passTurn();
		bool playAt(BoardPosition pos);
		
		IEnumerator<BoardPosition> GetEnumerator();
		BoardPosition getPosition(int row, int col);
		BoardValue getCellValue(BoardPosition pos);
		
		bool undo();
		bool redo();
		void reset();
		void goToEnd();
		void advance(int num);
		
		GoMove currentMove();
	}
	
	public interface GoGameListener {
		void setBoardPoint(BoardPosition pos, BoardValue value);
		void setData(BoardValue turn, int currentMoveNumber, int totalMoves, TeamData blackData, TeamData whiteData);
	}
	
	public class TeamData {
		public int prisoners;
		public int passes;
		
		public TeamData() {
			prisoners = 0;
			passes = 0;
		}
	}
	
	public class GoMove {
		public BoardValue turn;
		public BoardPosition pos; // null if it's a pass
		public List<BoardPosition> prisoners;
		
		public GoMove(BoardValue turn, BoardPosition pos) {
			this.turn = turn;
			this.pos = pos;
			this.prisoners = new List<BoardPosition>();
		}
		
		public void addPrisoner(BoardPosition c) {
			prisoners.Add(c);
		}
	}
	
	public class BoardPosition {
		public int row;
		public int col;
		
		static char[] letters = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't' };
		
		
		public BoardPosition() : this(0, 0) {
			// noop
		}
		public BoardPosition(int row, int col) {
			this.row = row;
			this.col = col;
		}
		public override string ToString() {
			if(row >= 0 && row < 19 && col >= 0 && col < 19) {
				return "" + letters[col] + "" + (row + 1);
			} else {
				string ret = col + "/" + row;
				UnityEngine.Debug.LogWarning("Uncommon position: " + ret);
				return ret;
			}
		}
		
		public static bool operator ==(BoardPosition b1, BoardPosition b2) {
			if (System.Object.ReferenceEquals(b1, b2)) {
				return true;
			}
			if(((object)b1 == null) || ((object)b2 == null)) {
				return false;
			}
			
			return b1.row == b2.row && b1.col == b2.col;
		}
		public static bool operator !=(BoardPosition b1, BoardPosition b2) {
			return !(b1 == b2);
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			
			BoardPosition other = obj as BoardPosition;
			
			if((object)other == null) {
				UnityEngine.Debug.Log("Is not a BoardPosition");
				return false;
			}
			
			return row == other.row && col == other.col;
		}
		
		public override int GetHashCode() {
			return row * 1000 + col;
		}
	}

	public enum BoardValue {
		Empty, White, Black
	}
	
}