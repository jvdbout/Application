﻿using WinEchek.Model;
using WinEchek.Model.Piece;

namespace WinEchek.Command
{
    public class Move : ICompensableCommand
    {
        public Piece Piece { get; internal set; }
        private Piece _removedPiece;
        public Square Square { get; internal set; }
        private Square _targetSquare;

        public Move(Piece piece, Square targetSquare)
        {
            Piece = piece;
            _targetSquare = targetSquare;
        }

        public void Execute()
        {
            Square = Piece.Square;

            if (_targetSquare.Piece == null)//Si case vide
            {
                Square.Piece = null;
                Piece.Square = _targetSquare;
                _targetSquare.Piece = Piece;
            }
            else
            {
                _removedPiece = _targetSquare.Piece;
                _targetSquare.Piece = null;
                Piece.Square.Piece = null;
                Piece.Square = _targetSquare;
                _targetSquare.Piece = Piece;
            }
        }

        public void Compensate()
        {
            _targetSquare.Piece = _removedPiece;
            Square.Piece = Piece;
            Piece.Square = Square;
        }
    }
}