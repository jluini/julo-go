
using System.Collections.Generic;

namespace JuloGo {
	
	public interface GoAnalyzer {
		void setGame(GoGame game);
		void analyze();
		
		int getBlackPoints();
		int getWhitePoints();
		
		Dictionary<BoardValue, TeamScore> getTeamScores();
		
		IEnumerable<BoardPosition> getDeadPositions();
		IEnumerable<BoardPosition> getBlackPositions();
		IEnumerable<BoardPosition> getWhitePositions();
	}
	
	public class TeamScore {
		public int stones;
		public int territory;
		
		public TeamScore() {
			stones = 0;
			territory = 0;
		}
		
		public int getArea() {
			return stones + territory;
		}
	}
	public class Result {
		public int blackPoints;
		public int whitePoints;
		
		public Result(int blackPoints, int whitePoints) {
			this.blackPoints = blackPoints;
			this.whitePoints = whitePoints;
		}
		public override string ToString() {
			return blackPoints + " vs " + whitePoints + " (black " + withSign(blackPoints - whitePoints) + ")";
		}
		
		static string withSign(int num) {
			if(num <= 0) {
				return num.ToString();
			} else {
				return "+" + num.ToString();
			}
		}
	}
}