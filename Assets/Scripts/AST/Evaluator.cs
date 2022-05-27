﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AST
{
    // Map: name -> [] or name -> Variable implemented by NoiseValue NoiseMapValue ColorValue
    public class Evaluator : ITilemapDSLVisitor
    {
        private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        private Dictionary<string, Function> functions = new Dictionary<string, Function>();

        public void visit(TilemapGenerator tilemapGenerator, Program p)
        {
            Canvas canvas = p.getCanvas();
            canvas.Accept(tilemapGenerator, this);
            foreach (Function function in p.getFunctions())
            {
                function.Accept(tilemapGenerator, this);
            }

            foreach (Statement statement in p.getStatements())
            {
                statement.Accept(tilemapGenerator, this);
            }
        }

        public void visit(TilemapGenerator tilemapGenerator, Call c)
        {
            // Get function from dictionary using it's name
            // Call function Execute method
            if (!functions.ContainsKey(c.getFunctionName())) {
                throw new Exception("Function does not exist");
            }
            Function function = functions[c.getFunctionName()];
            function.Execute(tilemapGenerator, this, c.getX(), c.getY());;
        }

        public void visit(TilemapGenerator tilemapGenerator, Statement c)
        {
            throw new System.NotImplementedException();
        }

        public void visit(TilemapGenerator tilemapGenerator, Canvas c)
        {
            tilemapGenerator.Canvas(c.getWidth(), c.getHeight());
        }

        public void visit(TilemapGenerator tilemapGenerator, Color c)
        {
            variables.Add(c.GetName(), c);
        }

        public void visit(TilemapGenerator tilemapGenerator, Fill f)
        {
            Byte b = Convert.ToByte(f.getColor().GetB());
            Byte g = Convert.ToByte(f.getColor().GetG());
            Byte r = Convert.ToByte(f.getColor().GetR());
            Color32 color = new Color32(255, b, g, r);
            tilemapGenerator.Fill(f.getX(), f.getY(), f.getWidth(), f.getHeight(), color);
        }

        public void visit(TilemapGenerator tilemapGenerator, Function f)
        {
            functions.Add(f.GetName(), f);
            f.SetScope(functions.Count); // this works as long as we never remove any functions from the dictionary
        }

        public void visit(TilemapGenerator tilemapGenerator, Loop l)
        {
            // Problem: this simple implementation allows two or more loops of either X or Y to be nested
            // which does not make sense. We need to restrict nesting to two loops only one over x and the other over y.
            for (int i = l.GetFrom(); i <= l.GetTo(); i += l.GetStep())
            {
                foreach (Statement statement in l.GetStatements())
                {
                    if (l.GetIterator() == Iterator.X)
                    {
                        statement.SetLoopX(i);
                    }
                    else
                    {
                        statement.SetLoopY(i);
                    }
                    statement.Accept(tilemapGenerator, this);
                }
            }
        }

        public void visit(TilemapGenerator tilemapGenerator, If i)
        {
            Variable variable = variables[i.GetNoiseVariable()];
            if (!(variable is Noise))
            {
                throw new Exception("If variable should be of Noise type");
            }

            Noise noise = variable as Noise;
            // Implement GetInt in Noise
            // i.SetNoiseValue(noise.GetInt());
            if (i.EvaluateCondition())
            {
                foreach (Statement statement in i.GetStatements())
                {
                    statement.Accept(tilemapGenerator, this);
                }
            }
        }

        public void visit(TilemapGenerator tilemapGenerator, Noise n)
        {
            variables.Add(n.GetName(), n);
        }

        public void visit(TilemapGenerator tilemapGenerator, NoiseMap n)
        {
            variables.Add(n.GetName(), n);
        }
    }
}