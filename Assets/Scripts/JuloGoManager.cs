
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;
using JuloAudio;

namespace JuloGo {
	
	public class JuloGoManager : MonoBehaviour, GoGameListener {
		public GoGameOptions options;
		
		public AudioClip moveSound;
		public AudioClip invalidMoveSound;
		public AudioClip passSound;
		
		int numRows { get { return options.numRows; } }
		int numCols { get { return options.numCols; } }
		
		GoGame game;
		GoBoard board;
		GoAnalyzer gameAnalyzer;
		
		float unitsPerBoardPoint = 1.0f;
		
		InputManager inputManager;
		SoundsPlayerBehaviour soundsPlayer;
		
		void Start() {
			inputManager = JuloFind.singleton<InputManager>();
			soundsPlayer = JuloFind.singleton<SoundsPlayerBehaviour>();
			board = JuloFind.singleton<GoBoard>();
			board.options = options;
			
			game = new GoGameImpl(numRows, numCols);
			board.init(game);
			game.addListener(this);
			gameAnalyzer = new GoAnalyzerImpl();
			gameAnalyzer.setGame(game);
			
			game.init();
		}
		
		void Update() {
			if(inputManager.mouseIsDown()) {
				BoardPosition pos = getPositionUnderMouse();
				if(pos != null && game.getCellValue(pos) == BoardValue.Empty) {
					if(game.playAt(pos)) {
						soundsPlayer.playClip(moveSound);
					} else {
						soundsPlayer.playClip(invalidMoveSound);
					}
				}
			} else if(inputManager.isDownKey("p")) {
				game.passTurn();
				soundsPlayer.playClip(passSound);
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
			} else if(inputManager.isDownKey("space")) {
				analyze();
			}
		}
		
		public void setBoardPoint(BoardPosition pos, BoardValue value) {
			board.setBoardPoint(pos, value);
		}
		public void setData(BoardValue turn, int currentMoveNumber, int totalMoves, TeamData blackData, TeamData whiteData) {
			gameAnalyzer.analyze();
			Dictionary<BoardValue, TeamScore> scores = gameAnalyzer.getTeamScores();
			int blackPoints = scores[BoardValue.Black].getArea();
			int whitePoints = scores[BoardValue.White].getArea();

			board.setPoints(blackPoints, whitePoints);
			board.setData(turn, currentMoveNumber, totalMoves, blackData, whiteData);
		}
		
		bool isControl() {
			return Application.isEditor || inputManager.isKey("left ctrl") || inputManager.isKey("right ctrl");
		}
		
		BoardPosition getPositionUnderMouse() {
			Vector2 mousePos = inputManager.getMousePosition();
			int r, c;
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			
			r = (int)Mathf.Round(worldPos.y / unitsPerBoardPoint + (numRows - unitsPerBoardPoint) / 2.0f);
			c = (int)Mathf.Round(worldPos.x / unitsPerBoardPoint + (numCols - unitsPerBoardPoint) / 2.0f);
			
			return game.getPosition(r, c);
		}
		
		void analyze() {
			board.mark(
				gameAnalyzer.getDeadPositions(),
				gameAnalyzer.getBlackPositions(),
				gameAnalyzer.getWhitePositions()
			);
		}
	}
	
}