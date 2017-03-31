
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;
using JuloAudio;

namespace JuloGo {
	
	public class JuloGoManager : MonoBehaviour, GoGameListener {
		public bool screenSpace = false;
		
		public JuloGoOptions options;
		
		int numRows { get { return options.numRows; } }
		int numCols { get { return options.numCols; } }
		
		GoGame game;
		
		float unitsPerBoardPoint = 1.0f;
		
		InputManager inputManager;
		SoundsPlayerBehaviour soundsPlayer;
		Text caption;
		Text numMovesText;
		Text blackPointsText;
		Text whitePointsText;
		
		Text blackAreaText;
		Text whiteAreaText;
		
		StoneRenderer mark;
		StoneRenderer[,] stoneRenderers;
		
		
		void Start() {
			inputManager = JuloFind.singleton<InputManager>();
			soundsPlayer = JuloFind.singleton<SoundsPlayerBehaviour>();
			
			caption = JuloFind.byName<Text>("Caption");
			numMovesText    = JuloFind.byName<Text>("NumMoves");
			blackPointsText = JuloFind.byName<Text>("BlackPoints");
			whitePointsText = JuloFind.byName<Text>("WhitePoints");
			
			blackAreaText = JuloFind.byName<Text>("BlackArea");
			whiteAreaText = JuloFind.byName<Text>("WhiteArea");
			
			initBoard();
			
			game = new GoGameImpl(numRows, numCols);
			game.addListener(this);
			
			mark = Instantiate(options.markPrefab);
			mark.gameObject.SetActive(false);
			
			
			game.init();
		}
		
		void Update() {
			if(inputManager.mouseIsDown()) {
				BoardPosition pos = getPositionUnderMouse();
				if(pos != null && game.getCellValue(pos) == BoardValue.Empty) {
					if(game.playAt(pos)) {
						soundsPlayer.playClip(options.moveSound);
					} else {
						soundsPlayer.playClip(options.invalidMoveSound);
					}
				}
			} else if(inputManager.isDownKey("p")) {
				game.passTurn();
				soundsPlayer.playClip(options.passSound);
			} else if((inputManager.isDownKey("r") && isControl()) || inputManager.isDownKey("home")) {
				game.reset();
			} else if(inputManager.isDownKey("end")) {
				game.goToEnd();
			} else if(inputManager.isDownKey("page up")) {
				game.advance(-10);
			} else if(inputManager.isDownKey("page down")) {
				game.advance(10);
			} else if(inputManager.isDownKey("left")) {
				game.undo();
			} else if(inputManager.isDownKey("right")) {
				game.redo();
			}
		}
		
		void updateMark() {
			GoMove current = game.currentMove();
			if(current == null || current.pos == null) {
				// TODO
				mark.gameObject.SetActive(false);
			} else {
				mark.locate(current.pos.row, current.pos.col, numRows, numCols);
				mark.gameObject.SetActive(true);
			}
		}
		
		bool isControl() {
			return Application.isEditor || inputManager.isKey("left ctrl") || inputManager.isKey("right ctrl");
		}
		
		BoardPosition getPositionUnderMouse() {
			Vector2 mousePos = inputManager.getMousePosition();
			int r, c;
			if(screenSpace) {
				throw new ApplicationException("Not implemented");
			} else {
				Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
				
				r = (int)Mathf.Round(worldPos.y / unitsPerBoardPoint + (numRows - unitsPerBoardPoint) / 2.0f);
				c = (int)Mathf.Round(worldPos.x / unitsPerBoardPoint + (numCols - unitsPerBoardPoint) / 2.0f);
			}
			
			return game.getPosition(r, c);
		}
		public void setBoardPoint(BoardPosition pos, BoardValue value) {
			stoneRenderers[pos.row, pos.col].setSprite(getSpriteFor(value));
		}
		
		public void setData(BoardValue turn, int currentMoveNumber, int totalMoves, TeamData blackData, TeamData whiteData) {
			caption.text = turn == BoardValue.Black ? "Juega negro" : "Juega blanco";
			numMovesText.text    = currentMoveNumber + "/" + totalMoves;
			blackPointsText.text = blackData.prisoners.ToString();
			whitePointsText.text = whiteData.prisoners.ToString();
			
			updateMark();
			
			Dictionary<BoardValue, TeamScore> scores = game.getTeamScores();
			TeamScore blackScore = scores[BoardValue.Black];
			TeamScore whiteScore = scores[BoardValue.White];
			
			blackAreaText.text = blackScore.getArea().ToString();
			whiteAreaText.text = whiteScore.getArea().ToString();
			
			//Result chineseResult  = new Result(blackScore.getArea(), whiteScore.getArea());
			//Result japaneseResult = new Result(blackScore.territory + blackData.prisoners, whiteScore.territory + whiteData.prisoners);
			//Result agaResult      = new Result(blackScore.territory + blackData.prisoners + whiteData.passes, whiteScore.territory + whiteData.prisoners + blackData.passes);
			
			//Debug.Log("--------------------------");
			//Debug.Log("Chinese:  " + chineseResult);
			//Debug.Log("Japanese: " + japaneseResult);
			//Debug.Log("AGA:      " + agaResult);
		}
		
		Sprite getSpriteFor(BoardValue value) {
			if(value == BoardValue.Empty) {
				return null;
			} else if(value == BoardValue.White) {
				return options.whiteSprite;
			} else if(value == BoardValue.Black) {
				return options.blackSprite;
			}
			throw new ApplicationException("Invalid case");
		}
		
		void initBoard() {
			stoneRenderers = new StoneRenderer[numRows, numCols];
			
			for(int r = 0; r < numRows; r++) {
				for(int c = 0; c < numCols; c++) {
					StoneRenderer newRenderer = Instantiate(options.stoneRendererPrefab);
					stoneRenderers[r, c] = newRenderer;
					newRenderer.transform.SetParent(this.transform);
					
					newRenderer.locate(r, c, numRows, numCols);
				}
			}
		}
	}
	
}