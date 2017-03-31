
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;

namespace JuloGo {
	
	public class GoBoard : MonoBehaviour, GoGameListener {
		public StoneRenderer stoneRendererPrefab;
		
		public Sprite blackSprite;
		public Sprite whiteSprite;
		
		public Sprite lastMoveMark;
		public Sprite deadMark;
		
		public Sprite whiteMark;
		public Sprite blackMark;
		public Sprite noneMark;
		
		
		[HideInInspector]
		public GoGameOptions options;
		
		Text caption;
		Text numMovesText;
		Text blackPointsText;
		Text whitePointsText;
		
		Text blackAreaText;
		Text whiteAreaText;
		
		StoneRenderer[,] stoneRenderers;
		List<StoneRenderer> markedStones;
		
		GoGame game;
		
		
		public void init(GoGame game) {
			caption = JuloFind.byName<Text>("Caption");
			numMovesText    = JuloFind.byName<Text>("NumMoves");
			blackPointsText = JuloFind.byName<Text>("BlackPoints");
			whitePointsText = JuloFind.byName<Text>("WhitePoints");
			
			blackAreaText = JuloFind.byName<Text>("BlackArea");
			whiteAreaText = JuloFind.byName<Text>("WhiteArea");
			
			markedStones = new List<StoneRenderer>();
			
			this.game = game;
			initBoard();
		}
		
		public void setBoardPoint(BoardPosition pos, BoardValue value) {
			getRenderer(pos).setSprite(getSpriteFor(value));
		}
		
		public void setData(BoardValue turn, int currentMoveNumber, int totalMoves, TeamData blackData, TeamData whiteData) {
			caption.text = turn == BoardValue.Black ? "Juega negro" : "Juega blanco";
			numMovesText.text    = currentMoveNumber + "/" + totalMoves;
			blackPointsText.text = blackData.prisoners.ToString();
			whitePointsText.text = whiteData.prisoners.ToString();
			
			updateMark();
			/*
			Dictionary<BoardValue, TeamScore> scores = game.getTeamScores();
			TeamScore blackScore = scores[BoardValue.Black];
			TeamScore whiteScore = scores[BoardValue.White];
			
			blackAreaText.text = blackScore.getArea().ToString();
			whiteAreaText.text = whiteScore.getArea().ToString();
			*/
			//Result chineseResult  = new Result(blackScore.getArea(), whiteScore.getArea());
			//Result japaneseResult = new Result(blackScore.territory + blackData.prisoners, whiteScore.territory + whiteData.prisoners);
			//Result agaResult      = new Result(blackScore.territory + blackData.prisoners + whiteData.passes, whiteScore.territory + whiteData.prisoners + blackData.passes);
			
			//Debug.Log("--------------------------");
			//Debug.Log("Chinese:  " + chineseResult);
			//Debug.Log("Japanese: " + japaneseResult);
			//Debug.Log("AGA:      " + agaResult);
		}
		
		public void mark(IEnumerable<BoardPosition> deadPositions, IEnumerable<BoardPosition> blackPositions, IEnumerable<BoardPosition> whitePositions) {
			clearMarks();
			setMarkToAll(deadPositions,  noneMark);
			setMarkToAll(blackPositions, blackMark);
			setMarkToAll(whitePositions, whiteMark);
		}
		
		public void setPoints(int blackPoints, int whitePoints) {
			blackAreaText.text = blackPoints.ToString();
			whiteAreaText.text = whitePoints.ToString();
		}
		
		void initBoard() {
			stoneRenderers = new StoneRenderer[options.numRows, options.numCols];
			
			for(int r = 0; r < options.numRows; r++) {
				for(int c = 0; c < options.numCols; c++) {
					StoneRenderer newRenderer = Instantiate(stoneRendererPrefab);
					stoneRenderers[r, c] = newRenderer;
					newRenderer.transform.SetParent(this.transform);
					newRenderer.locate(r, c, options.numRows, options.numCols);
					newRenderer.setSprite(null);
					newRenderer.setMark(null);
				}
			}
		}
		
		void updateMark() {
			clearMarks();
			
			GoMove current = game.currentMove();
			if(current == null || current.pos == null) {
				// ...
			} else {
				StoneRenderer rend = getRenderer(current.pos);
				rend.setMark(lastMoveMark);
				markedStones.Add(rend);
				
				foreach(BoardPosition pp in current.prisoners) {
					StoneRenderer prisonerRendererer = stoneRenderers[pp.row, pp.col];
					prisonerRendererer.setMark(deadMark);
					markedStones.Add(prisonerRendererer);
				}
			}
		}
		
		void clearMarks() {
			while(markedStones.Count > 0) {
				markedStones[markedStones.Count - 1].setMark(null);
				markedStones.RemoveAt(markedStones.Count - 1);
			}
		}
		void setMarkToAll(IEnumerable<BoardPosition> positions, Sprite mark) {
			foreach(BoardPosition p in positions) {
				StoneRenderer rend = getRenderer(p);
				rend.setMark(mark);
				markedStones.Add(rend);
			}
		}
		
		StoneRenderer getRenderer(BoardPosition pos) {
			return stoneRenderers[pos.row, pos.col];
		}
		
		Sprite getSpriteFor(BoardValue value) {
			if(value == BoardValue.Empty) {
				return null;
			} else if(value == BoardValue.White) {
				return whiteSprite;
			} else if(value == BoardValue.Black) {
				return blackSprite;
			}
			throw new ApplicationException("Invalid case");
		}
	}
}