﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Parser
{
    public class MutationVisitor : NodeVisitor
    {
        private Random _random;
        private int _maxExplorationCount;
        private int _currentExplorationCount;
        private double _mutationRate;
        private int _mutationPlace;
        private NodeCreator _nodeCreator;
        private Node _rootNode;
        private int _maxCount;
        private int _maxDepth;
        private bool _hasMutation;

        public MutationVisitor(double mutationRate = 0.05, int maxDepth = 50, int maxSize = 500, int? seed = 7)
        {
            Init(mutationRate, maxDepth, maxSize, seed);
        }

        public void Mutate(ref Node n)
        {
            Reset();
            _rootNode = n;
            _maxExplorationCount = n.Count;
            if (_hasMutation)
            {
                _mutationPlace = _random.Next(0, _maxExplorationCount);
                if (_mutationPlace == _currentExplorationCount)
                {
                    n = _nodeCreator.RandomNode(_maxDepth, _maxCount);
                    return;
                }
            }
            VisitNode(n);
        }

        public override void Visit(BinaryNode n)
        {
            if (!_hasMutation || _mutationPlace < _currentExplorationCount) return;
            Explore(n);
            if (_mutationPlace == _currentExplorationCount)
            {
                MutateBinaryNode(n);
            }
            VisitNode(n.Left);
            VisitNode(n.Right);
        }

        public override void Visit(ConstantNode n)
        {
            if (!_hasMutation || _mutationPlace < _currentExplorationCount) return;
            Explore(n);
            if (_mutationPlace == _currentExplorationCount)
            {
                MutateConstantNode(n);
            }
        }

        public override void Visit(IfNode n)
        {
            if (!_hasMutation || _mutationPlace < _currentExplorationCount) return;
            Explore(n);
            if (_mutationPlace == _currentExplorationCount)
            {
                MutateIfNode(n);
            }
            VisitNode(n.Condition);
            VisitNode(n.WhenTrue);
            VisitNode(n.WhenFalse);
        }

        public override void Visit(UnaryNode n)
        {
            if (!_hasMutation || _mutationPlace < _currentExplorationCount) return;
            Explore(n);
            if (_mutationPlace == _currentExplorationCount)
            {
                MutateUnaryNode(n);
            }
            VisitNode(n.Right);
        }

        public void Reset()
        {
            _hasMutation = _random.NextDouble() <= _mutationRate;
            _maxExplorationCount = 0;
            _currentExplorationCount = 0;
            _rootNode = null;
        }

        private void Init(double mutationRate, int maxDepth, int maxCount, int? seed)
        {
            _mutationRate = mutationRate;
            _maxCount = maxCount;
            _maxDepth = maxDepth;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _hasMutation = _random.NextDouble() <= _mutationRate;
            _nodeCreator = new NodeCreator(_random);
        }

        private void Explore(Node n)
        {
            _currentExplorationCount++;
        }

        private void MutateBinaryNode(BinaryNode n)
        {
            if (PossibleDepth(n) < 1 || PossibleCount(n) < 1) return;
            int kind = _random.Next(3);
            switch (kind)
            {
                case 0:
                    n.OperatorType = _nodeCreator.RandomBinaryNodeOperator();
                    break;

                case 1:
                    n.Left = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                case 2:
                    n.Right = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                default:
                    throw new InvalidOperationException("Not the right kind");
            }
        }

        private void MutateConstantNode(ConstantNode n)
        {
            n.Value = _nodeCreator.RandomConstantNode().Value;
        }

        private void MutateIfNode(IfNode n)
        {
            if (PossibleDepth(n) < 1 || PossibleCount(n) < 1) return;
            int kind = _random.Next(3);
            switch (kind)
            {
                case 0:
                    n.Condition = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                case 1:
                    n.WhenTrue = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                case 2:
                    n.WhenFalse = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                default:
                    throw new InvalidOperationException("Not the right kind");
            }
        }

        private void MutateUnaryNode(UnaryNode n)
        {
            if (PossibleDepth(n) < 1 || PossibleCount(n) < 1) return;
            int kind = _random.Next(2);
            switch (kind)
            {
                case 0:
                    n.OperatorType = _nodeCreator.RandomUnaryNodeOperator();
                    break;

                case 1:
                    n.Right = _nodeCreator.RandomNode(PossibleDepth(n), PossibleCount(n));
                    break;

                default:
                    throw new InvalidOperationException("Not the right kind");
            }
        }

        private int PossibleDepth(Node n)
        {
            return _maxDepth - _rootNode.Depth + n.Depth;
        }

        private int PossibleCount(Node n)
        {
            return _maxCount - _rootNode.Count + n.Count;
        }
    }
}