using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Wpf.Messages;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestDataViewModel : PropertyChangedBase
    {
        #region Members

        private bool adjustZeroPoint;
        private double calcLineSlope;
        private readonly CbrTestData testData;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region Properties

        public double Slope
        {
            get => testData.TestResults.Slope;
            set
            {
                testData.TestResults.Slope = value;
                NotifyOfPropertyChange(() => Slope);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double MaxSlope { get; private set; }

        public double MinSlope { get; private set; }

        public double Intercept
        {
            get => testData.TestResults.Intercept;
            set
            {
                testData.TestResults.Intercept = value;
                NotifyOfPropertyChange(() => Intercept);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double ZeroPoint
        {
            get
            {
                if (AdjustZeroPoint)
                {
                    return -Intercept / Slope;
                }
                return 0.0d;
            }
        }

        public double SplineRho
        {
            get => testData.TestResults.SplineRho;
            set
            {
                testData.TestResults.SplineRho = value;
                NotifyOfPropertyChange(() => SplineRho);
            }
        }

        public double SplineNodes
        {
            get => testData.TestResults.SplineNodes;
            set
            {
                testData.TestResults.SplineNodes = value;
                NotifyOfPropertyChange(() => SplineNodes);
            }
        }

        public double Force1
        {
            get => testData.TestResults.Force1;
            set
            {
                testData.TestResults.Force1 = value;
                NotifyOfPropertyChange(() => Force1);
            }
        }

        public double Force2
        {
            get => testData.TestResults.Force2;
            set
            {
                testData.TestResults.Force2 = value;
                NotifyOfPropertyChange(() => Force2);
            }
        }

        public Boolean AdjustZeroPoint
        {
            get => adjustZeroPoint;
            set
            {
                if (value != adjustZeroPoint)
                {
                    adjustZeroPoint = value;
                    NotifyOfPropertyChange(() => ZeroPoint);
                    NotifyOfPropertyChange(() => AdjustZeroPoint);
                }
            }
        }

        public string DisplayName => testData?.Name;

        public IList<ITestPoint> TestDataPoints => testData?.TestDataPoints;

        #endregion

        #region Constructors

        public CbrTestDataViewModel(CbrTestData testData, IEventAggregator eventAggregator)
        {
            this.testData = testData;
            this.eventAggregator = eventAggregator;

            if (Math.Abs(testData.TestResults.Slope) > double.Epsilon && 
                Math.Abs(testData.TestResults.Intercept) > double.Epsilon)
            {
                AdjustZeroPoint = true;
            }

            FillSlopeRangeFromDataPoints();
        }

        #endregion

        private void FillSlopeRangeFromDataPoints()
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            var sum = 0.0;

            for (var i = 0; i < testData.TestDataPoints.Count - 1; i++)
            {
                var p1 = testData.TestDataPoints[i + 0];
                var p2 = testData.TestDataPoints[i + 1];

                var result = Math.Round(Math.Abs(p1.Force - p2.Force) / Math.Abs(p1.Stroke - p2.Stroke), 2);

                if (result > max) max = result;
                if (result < min) min = result;

                sum += result;
            }

            calcLineSlope = sum / (testData.TestDataPoints.Count - 1);

            MinSlope = min;
            MaxSlope = max;
        }

        #region Overridden Methods

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            {
                switch (propertyName)
                {
                    case "Slope":
                    case "Intercept":
                    case "SplineRho":
                    case "SplineNodes":
                        eventAggregator.PublishOnUIThread(PlotMessages.InvalidatePlot);
                        break;
                    case "AdjustZeroPoint":
                        {
                            if (AdjustZeroPoint)
                            {
                                if (Math.Abs(Slope) < double.Epsilon)
                                {
                                    Slope = calcLineSlope;
                                }
                            }
                            else
                            {
                                Slope = 0;
                                Intercept = 0;
                            }
                            eventAggregator.PublishOnUIThread(PlotMessages.InvalidatePlot);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}