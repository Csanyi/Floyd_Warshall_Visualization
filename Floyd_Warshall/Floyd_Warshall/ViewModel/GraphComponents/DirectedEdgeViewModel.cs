﻿using System;
using System.ComponentModel;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
	/// <summary>
	/// Type of the directed edge viewmodel
	/// </summary>
	public class DirectedEdgeViewModel : EdgeViewModelBase
    {
        private readonly int _vertexRadius = VertexViewModel.Size / 2;
        private const double _angleOffset = 0.3;                      
        private const int _endPointOffset = 5;

        /// <summary>
        /// Constructor of the directed edge viewmodel
        /// </summary>
        /// <param name="id">The edge id</param>
        /// <param name="graphModel">The graph model</param>
        /// <param name="from">The start vertex viewmodel of the edge</param>
        /// <param name="to">The end vertex viewmodel of the edge></param>
        public DirectedEdgeViewModel(int id, GraphModel graphModel, VertexViewModel from, VertexViewModel to) 
                                            : base(id, graphModel, from, to) { }

        public override double X1
        {
            get
            {
                double x1 = From.GetX() - CanvasX;
                double radian = AngleInRadian;
                double offset = _vertexRadius * Math.Cos(radian - _angleOffset);

                if(radian == Math.PI / 2)
                {
                    return (From.CanvasY < To.CanvasY) ? x1 + offset : x1 - offset;
                } 
                else
                {
                    return (From.CanvasX < To.CanvasX) ? x1 + offset : x1 - offset;
                }
            }
        }

        public override double Y1
        {
            get
            {
                double y1 = From.GetY() - CanvasY;
                double radian = AngleInRadian;
                double offset = _vertexRadius * Math.Sin(radian - _angleOffset);

                if(radian == 0.0)
                {
                    return (From.CanvasX < To.CanvasX) ? y1 + offset : y1 - offset;
                }
                else
                {
                    return (From.CanvasY < To.CanvasY) ? y1 + offset : y1 - offset;
                }
            }
        }

        public override double X2
        {
            get
            {
                double x2 = To.GetX() - CanvasX;
                double radian = AngleInRadian;
                double offset = (_vertexRadius + _endPointOffset) * Math.Cos(radian + _angleOffset);

                if(radian == Math.PI / 2)
                {
                    return (From.CanvasY < To.CanvasY) ? x2 - offset : x2 + offset;
                }
                else
                {
                    return (From.CanvasX < To.CanvasX) ? x2 - offset : x2 + offset;

                }
            }
        }

        public override double Y2
        {
            get
            {
                double y2 = To.GetY() - CanvasY;
                double radian = AngleInRadian;
                double offset = (_vertexRadius + _endPointOffset) * Math.Sin(radian + _angleOffset);

                if(radian == 0.0)
                {
                    return (From.CanvasX < To.CanvasX) ? y2 - offset : y2 + offset;
                }
                else 
                {
                    return (From.CanvasY < To.CanvasY) ? y2 - offset : y2 + offset;
                }
            }
        }

        /// <summary>
        /// Gets the edge's angle in radian
        /// </summary>
        public double AngleInRadian 
        { 
            get 
            { 
                if (Width <= 0.001)
                {
                    return Math.PI / 2;
                } else
                {
                    return Math.Atan(Height / Width);
                }
            } 
        }

        /// <summary>
        /// Gets the edge's angle in degree
        /// </summary>
        public double Angle {
            get
            {
                double angle = AngleInRadian * (180 / Math.PI);

                if (From.CanvasX >= To.CanvasX && From.CanvasY <= To.CanvasY)
                {
                    angle = 180 - angle;
                }
                else if (From.CanvasX >= To.CanvasX && From.CanvasY >= To.CanvasY)
                {
                    angle += 180;
                }
                else if (From.CanvasX <= To.CanvasX && From.CanvasY >= To.CanvasY)
                {
                    angle = 360 - angle;
                }

                return angle;
            }
        }

        /// <summary>
        /// Gets the edge's width
        /// </summary>
        private double Width { get { return Math.Abs(From.CanvasX - To.CanvasX); } }

        /// <summary>
        /// Gets the edge's height
        /// </summary>
        private double Height { get { return Math.Abs(From.CanvasY - To.CanvasY); } }

        /// <summary>
        /// Gets the edge's text x coord
        /// </summary>
        public double TextX { get { return (X1 + X2) / 2; } }

        /// <summary>
        /// Gets the edge's text y coord
        /// </summary>
        public double TextY { get { return (Y1 + Y2) / 2; } }

        protected override void VertexPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            base.VertexPropertyChanged(sender, e);

            if (e.PropertyName == nameof(VertexViewModel.CanvasX) || e.PropertyName == nameof(VertexViewModel.CanvasY))
            {
                OnPropertyChanged(nameof(Angle));
                OnPropertyChanged(nameof(TextX));
                OnPropertyChanged(nameof(TextY));
            }
        }

        protected override void Model_EdgeUpdated(object? sender, EdgeUpdatedEventArgs e)
        {
            if(e.From == From.Id && e.To == To.Id)
            {
                OnPropertyChanged(nameof(Weight));
                OnEdgeUpdated();
            }
        }
    }
}
