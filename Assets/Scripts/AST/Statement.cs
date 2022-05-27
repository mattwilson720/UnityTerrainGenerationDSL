﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AST
{
    public abstract class Statement : Element
    {
        protected Vector2Int positionOffset;
        protected int loopX;
        protected int loopY;
        
        public void SetPositionOffset(int x, int y) {
            positionOffset = new Vector2Int(x, y);
        }

        public Vector2Int GetPositionOffset() {
            return positionOffset;
        }
        public void SetLoopX(int x) 
        {
            loopX = x;
        }
        
        public void SetLoopY(int y) 
        {
            loopY = y;
        }
        
        public int GetLoopX() {
            return loopX;
        }
        
        public int GetLoopY() {
            return loopY;
        }
    }
}