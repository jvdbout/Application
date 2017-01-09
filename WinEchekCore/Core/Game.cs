﻿using System.Collections.Generic;
using WinEchek.Engine;
using WinEchek.Model;
using WinEchek.Model.Pieces;

namespace WinEchek.Core
{
    public class Game
    {
        private Player _currentPlayer;

        private Player WhitePlayer { get; }
        private Player BlackPlayer { get; }
        private IEngine Engine { get; }
        public Container Container { get; set; }

        /// <summary>
        /// Construct a game with an engine and two players
        /// </summary>
        /// <param name="engine">The engin the game will use</param>
        /// <param name="whitePlayer">White player</param>
        /// <param name="blackPlayer">Black player</param>
        /// <param name="container">Model container</param>
        public Game(IEngine engine, Player whitePlayer, Player blackPlayer, Container container)
        {
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
            Engine = engine;
            Container = container;

            WhitePlayer.MoveDone += PlayerMoveHandler;
            BlackPlayer.MoveDone += PlayerMoveHandler;

            _currentPlayer = WhitePlayer;
            OnBoardStateChanged();

            _currentPlayer.Play(null);
        }

        /// <summary>
        /// Délégué appelé quand un joueur réalise un coup.
        /// </summary>
        /// <remarks>
        /// On vérifie si le coup est valide et si c'est le cas on demande à l'autre joueur de jouer.
        /// Sinon on indique au joueur que le coup est invalide afin qu'il nous redonne un coup.
        /// On réalise ces actions tant que la partie n'est pas echec et mat ou echec et pat.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="move"></param>
        private void PlayerMoveHandler(Player sender, Move move)
        {
            if (sender != _currentPlayer)
            {
                sender.Stop();
            }
            else
            {
                if (Engine.DoMove(move))
                {
                    _currentPlayer.Stop();
                    ChangePlayer();
                    OnBoardStateChanged();
                }

                _currentPlayer.Play(move);
            }
        }

        private void ChangePlayer() => _currentPlayer = _currentPlayer == WhitePlayer ? BlackPlayer : WhitePlayer;

        public List<Square> PossibleMoves(Piece piece) => Engine.PossibleMoves(piece);

        #region Undo Redo

        /// <summary>
        /// Demande au moteur d'annuler le dernier coup joué
        /// </summary>
        public void Undo()
        {
            Move move = Engine.Undo();
            if (move == null) return;

            _currentPlayer.Stop();
            ChangePlayer();
            OnBoardStateChanged();
            _currentPlayer.Play(move);
        }

        public void Undo(int count)
        {
            Move lastMove = null;
            for (int i = 0; i < count; i++)
            {
                Move move = Engine.Undo();
                if (move != null)
                {
                    _currentPlayer.Stop();
                    ChangePlayer();
                    _currentPlayer.Play(move);
                    lastMove = move;
                }
            }

            if(lastMove != null)
                OnBoardStateChanged();
        }

        /// <summary>
        /// Demande au moteur de refaire le dernier coup annulé
        /// </summary>
        public void Redo()
        {
            Move move = Engine.Redo();
            if (move == null) return;

            _currentPlayer.Stop();
            ChangePlayer();
            StateChanged?.Invoke(Engine.CurrentState());
            _currentPlayer.Play(move);
        }

        #endregion

        #region Delegate and Events

        public delegate void StateHandler(BoardState state);
        public event StateHandler StateChanged;
        private void OnBoardStateChanged() => StateChanged?.Invoke(Engine.CurrentState());

        #endregion
    }
}