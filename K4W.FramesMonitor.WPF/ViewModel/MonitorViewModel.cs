using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Kinect;

namespace K4W.FramesMonitor.WPF.ViewModel
{
    public class MonitorViewModel : ViewModelBase
    {
        /// <summary>
        /// Kinect-sensor
        /// </summary>
        private KinectSensor _kinect;

        /// <summary>
        /// Kinect MultiSourceFrameReader
        /// </summary>
        private MultiSourceFrameReader _multiFrameReader;

        /// <summary>
        /// FPS limit (/second)
        /// </summary>
        private DateTime _newLimit = new DateTime(2000, 1, 1);

        /// <summary>
        /// Global frame counter
        /// </summary>
        private int _globalCounter = 0;

        /// <summary>
        /// Color frame counter
        /// </summary>
        private int _colorCounter = 0;

        /// <summary>
        /// Depth frame counter
        /// </summary>
        private int _depthCounter = 0;

        /// <summary>
        /// Body frame counter
        /// </summary>
        private int _bodyCounter = 0;

        /// <summary>
        /// Body index frame counter
        /// </summary>
        private int _bodyIndexCounter = 0;

        /// <summary>
        /// Infrared frame counter
        /// </summary>
        private int _infraredCounter = 0;

        /// <summary>
        /// Long exposure infrared frame counter
        /// </summary>
        private int _longExposureCounter = 0;


        /// <summary>
        /// Default CTOR
        /// </summary>
        public MonitorViewModel()
        {
            InitializeKinect();
        }


        /// <summary>
        /// Initialize Kinect-sensor
        /// </summary>
        private void InitializeKinect()
        {
            // Get the default sensor
            _kinect = KinectSensor.Default;

            if (_kinect == null || _kinect.Status != KinectStatus.Connected) return;

            // Start using the Kinect-sensor
            _kinect.Open();
        }

        /// <summary>
        /// Start monitoring
        /// </summary>
        private void StartMonitor()
        {
            // Construct the requested sourcetypes
            FrameSourceTypes sourceTypes = ConstructSourceType();

            // Return if no sources are selected
            if (sourceTypes == 0)
                return;

            // Change UI
            IsMonitorRunning = true;

            // Reset monitor values
            ResetMonitor();

            // Open new MultiSourceFrameReader for the selected sources
            _multiFrameReader = _kinect.OpenMultiSourceFrameReader(sourceTypes);

            // Hook up to the event
            _multiFrameReader.MultiSourceFrameArrived += OnFramesArrived;
        }

        /// <summary>
        /// Reset all monitor values
        /// </summary>
        private void ResetMonitor()
        {
            if (TrackColor == true)
            {
                CurrentColor = "0";
                FPSColor = CurrentColor;
            }
            else
            {
                CurrentColor = "N/A";
                FPSColor = CurrentColor;
            }

            if (TrackDepth == true)
            {
                CurrentDepth = "0";
                FPSDepth = CurrentDepth;
            }
            else
            {
                CurrentDepth = "N/A";
                FPSDepth = CurrentDepth;
            }

            if (TrackBody == true)
            {
                CurrentBody = "0";
                FPSBody = CurrentBody;
            }
            else
            {
                CurrentBody = "N/A";
                FPSBody = CurrentBody;
            }

            if (TrackBodyIndex == true)
            {
                CurrentBodyIndex = "0";
                FPSBodyIndex = CurrentBodyIndex;
            }
            else
            {
                CurrentBodyIndex = "N/A";
                FPSBodyIndex = CurrentBodyIndex;
            }

            if (TrackInfrared == true)
            {
                CurrentInfrared = "0";
                FPSInfrared = CurrentInfrared;
            }
            else
            {
                CurrentInfrared = "N/A";
                FPSInfrared = CurrentInfrared;
            }

            if (TrackLongExposure == true)
            {
                CurrentLongExposed = "0";
                FPSLongExposed = CurrentLongExposed;
            }
            else
            {
                CurrentLongExposed = "N/A";
                FPSLongExposed = CurrentLongExposed;
            }

            CurrentTotal = "0";
            FPSTotal = "0";
        }

        /// <summary>
        /// Construct FrameSourceTypes-enumeration with the values for the selected sources
        /// </summary>
        private FrameSourceTypes ConstructSourceType()
        {
            FrameSourceTypes result = FrameSourceTypes.None;

            if (TrackColor == true) result |= FrameSourceTypes.Color;
            if (TrackDepth == true) result |= FrameSourceTypes.Depth;
            if (TrackBody == true) result |= FrameSourceTypes.Body;
            if (TrackBodyIndex == true) result |= FrameSourceTypes.BodyIndex;
            if (TrackInfrared == true) result |= FrameSourceTypes.Infrared;
            if (TrackLongExposure == true) result |= FrameSourceTypes.LongExposureInfrared;

            return result;
        }

        /// <summary>
        /// Stop monitoring
        /// </summary>
        private void StopMonitor()
        {
            // Change UI
            IsMonitorRunning = false;

            // Reset second-limit
            _newLimit = new DateTime(2000, 1, 1);

            // Remove frame-handler
            _multiFrameReader.MultiSourceFrameArrived -= OnFramesArrived;

            // Get rid of the reader
            _multiFrameReader.Dispose();
        }

        /// <summary>
        /// Process all inbound frames
        /// </summary>
        private void OnFramesArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Get multisourceframe reference
            MultiSourceFrameReference multiFrameRef = e.FrameReference;

            if (multiFrameRef == null) return;

            // Acquire new multisource frame
            using (var multiFrame = multiFrameRef.AcquireFrame())
            {
                if (multiFrame == null) return;

                // Set new timeframe if year = 2000 (Default value)
                if (_newLimit.Year == 2000)
                    _newLimit = DateTime.Now.AddSeconds(1);

                // Visualize FPS if timeframe has passed
                if (_newLimit < DateTime.Now)
                {
                    // Visualize global FPS
                    FPSTotal = _globalCounter.ToString();

                    // Visualize FPS if tracked
                    if (TrackColor == true) FPSColor = _colorCounter.ToString();
                    if (TrackDepth == true) FPSDepth = _depthCounter.ToString();
                    if (TrackBody == true) FPSBody = _bodyCounter.ToString();
                    if (TrackBodyIndex == true) FPSBodyIndex = _bodyIndexCounter.ToString();
                    if (TrackInfrared == true) FPSInfrared = _infraredCounter.ToString();
                    if (TrackLongExposure == true) FPSLongExposed = _longExposureCounter.ToString();

                    // Set new timeframe
                    _newLimit = DateTime.Now.AddSeconds(1);

                    // Reset counter
                    _globalCounter = 0;
                    _colorCounter = 0;
                    _depthCounter = 0;
                    _bodyCounter = 0;
                    _bodyIndexCounter = 0;
                    _infraredCounter = 0;
                    _longExposureCounter = 0;
                }

                bool validFrame = true;

                // Process color frame if trakced
                if (TrackColor == true)
                {
                    // Get frame reference
                    ColorFrameReference colorFrameRef = multiFrame.ColorFrameReference;

                    // Acquire new frame
                    using (ColorFrame colorFrame = colorFrameRef.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (colorFrame != null)
                        {
                            _colorCounter++;
                            CurrentColor = (Int32.Parse(CurrentColor) + 1).ToString();
                        }
                        else validFrame = false;

                    }
                }

                // Process depth frame if trakced
                if (TrackDepth == true)
                {
                    // Get frame reference
                    DepthFrameReference depthReference = multiFrame.DepthFrameReference;

                    // Acquire new frame
                    using (DepthFrame depthFrame = depthReference.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (depthFrame != null)
                        {
                            _depthCounter++;
                            CurrentDepth = (Int32.Parse(CurrentDepth) + 1).ToString();
                        }
                        else
                            validFrame = false;
                    }
                }

                // Process body frame if trakced
                if (TrackBody == true)
                {
                    // Get frame reference
                    BodyFrameReference bodyReference = multiFrame.BodyFrameReference;

                    // Acquire new frame
                    using (BodyFrame bodyFrame = bodyReference.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (bodyFrame != null)
                        {
                            _bodyCounter++;
                            CurrentBody = (Int32.Parse(CurrentBody) + 1).ToString();
                        }
                        else validFrame = false;
                    }
                }

                // Process body index frame if trakced
                if (TrackBodyIndex == true)
                {
                    // Get frame reference
                    BodyIndexFrameReference bodyReference = multiFrame.BodyIndexFrameReference;

                    // Acquire new frame
                    using (BodyIndexFrame indexFrame = bodyReference.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (indexFrame != null)
                        {
                            _bodyIndexCounter++;
                            CurrentBodyIndex = (Int32.Parse(CurrentBodyIndex) + 1).ToString();
                        }
                        else validFrame = false;
                    }
                }

                // Process infrared frame if trakced
                if (TrackInfrared == true)
                {
                    // Get frame reference
                    InfraredFrameReference infraReference = multiFrame.InfraredFrameReference;

                    // Acquire new frame
                    using (InfraredFrame infraFrame = infraReference.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (infraFrame != null)
                        {
                            _infraredCounter++;
                            CurrentInfrared = (Int32.Parse(CurrentInfrared) + 1).ToString();
                        }
                        else validFrame = false;
                    }
                }

                // Process long exposure infrared frame if trakced
                if (TrackLongExposure == true)
                {
                    // Get frame reference
                    LongExposureInfraredFrameReference longExposureReference = multiFrame.LongExposureInfraredFrameReference;

                    // Acquire new frame
                    using (LongExposureInfraredFrame exposureFrame = longExposureReference.AcquireFrame())
                    {
                        // Increment counter & update UI
                        if (exposureFrame != null)
                        {
                            _longExposureCounter++;
                            CurrentLongExposed = (Int32.Parse(CurrentLongExposed) + 1).ToString();
                        }
                        else validFrame = false;
                    }
                }

                // Increment global counter if in timeframe
                if ((_newLimit >= DateTime.Now) && validFrame == true)
                {
                    _globalCounter++;
                    CurrentTotal = (Int32.Parse(CurrentTotal) + 1).ToString();
                }
            }
        }

        #region TRACKING PROPERTIES
        public const string TrackBodyIndexPropertyName = "TrackBodyIndex";
        public const string TrackBodyPropertyName = "TrackBody";
        public const string TrackColorPropertyName = "TrackColor";
        public const string TrackDepthPropertyName = "TrackDepth";
        public const string TrackInfraredPropertyName = "TrackInfrared";
        public const string TrackLongExposurePropertyName = "TrackLongExposure";
        private bool _trackBody = true;
        private bool _trackBodyIndex = true;
        private bool _trackColor = true;
        private bool _trackDepth = true;
        private bool _trackInfrared = true;
        private bool _trackLongExposure = true;

        public bool TrackBody
        {
            get
            {
                return _trackBody;
            }

            set
            {
                if (_trackBody == value)
                {
                    return;
                }

                _trackBody = value;
                RaisePropertyChanged(TrackBodyPropertyName);
            }
        }

        public bool TrackBodyIndex
        {
            get
            {
                return _trackBodyIndex;
            }

            set
            {
                if (_trackBodyIndex == value)
                {
                    return;
                }

                _trackBodyIndex = value;
                RaisePropertyChanged(TrackBodyIndexPropertyName);
            }
        }

        public bool TrackColor
        {
            get
            {
                return _trackColor;
            }

            set
            {
                if (_trackColor == value)
                {
                    return;
                }

                _trackColor = value;
                RaisePropertyChanged(TrackColorPropertyName);
            }
        }
        public bool TrackDepth
        {
            get
            {
                return _trackDepth;
            }

            set
            {
                if (_trackDepth == value)
                {
                    return;
                }

                _trackDepth = value;
                RaisePropertyChanged(TrackDepthPropertyName);
            }
        }
        public bool TrackInfrared
        {
            get
            {
                return _trackInfrared;
            }

            set
            {
                if (_trackInfrared == value)
                {
                    return;
                }

                _trackInfrared = value;
                RaisePropertyChanged(TrackInfraredPropertyName);
            }
        }
        public bool TrackLongExposure
        {
            get
            {
                return _trackLongExposure;
            }

            set
            {
                if (_trackLongExposure == value)
                {
                    return;
                }

                _trackLongExposure = value;
                RaisePropertyChanged(TrackLongExposurePropertyName);
            }
        }
        #endregion TRACKING PROPERTIES

        #region CURRENT-VALUE PROPERTIES
        public const string CurrentBodyIndexPropertyName = "CurrentBodyIndex";
        public const string CurrentBodyPropertyName = "CurrentBody";
        public const string CurrentColorPropertyName = "CurrentColor";
        public const string CurrentDepthPropertyName = "CurrentDepth";
        public const string CurrentInfraredPropertyName = "CurrentInfrared";
        public const string CurrentLongExposedPropertyName = "CurrentLongExposed";
        public const string CurrentTotalPropertyName = "CurrentTotal";
        private string _currentBody = "-";
        private string _currentBodyIndex = "-";
        private string _currentColor = "-";
        private string _currentDepth = "-";
        private string _currentInfrared = "-";
        private string _currentLongExposed = "-";
        private string _currentTotal = "-";

        public string CurrentBody
        {
            get
            {
                return _currentBody;
            }

            set
            {
                if (_currentBody == value)
                {
                    return;
                }

                _currentBody = value;
                RaisePropertyChanged(CurrentBodyPropertyName);
            }
        }

        public string CurrentBodyIndex
        {
            get
            {
                return _currentBodyIndex;
            }

            set
            {
                if (_currentBodyIndex == value)
                {
                    return;
                }

                _currentBodyIndex = value;
                RaisePropertyChanged(CurrentBodyIndexPropertyName);
            }
        }

        public string CurrentColor
        {
            get
            {
                return _currentColor;
            }

            set
            {
                if (_currentColor == value)
                {
                    return;
                }

                _currentColor = value;
                RaisePropertyChanged(CurrentColorPropertyName);
            }
        }
        public string CurrentDepth
        {
            get
            {
                return _currentDepth;
            }

            set
            {
                if (_currentDepth == value)
                {
                    return;
                }

                _currentDepth = value;
                RaisePropertyChanged(CurrentDepthPropertyName);
            }
        }
        public string CurrentInfrared
        {
            get
            {
                return _currentInfrared;
            }

            set
            {
                if (_currentInfrared == value)
                {
                    return;
                }

                _currentInfrared = value;
                RaisePropertyChanged(CurrentInfraredPropertyName);
            }
        }
        public string CurrentLongExposed
        {
            get
            {
                return _currentLongExposed;
            }

            set
            {
                if (_currentLongExposed == value)
                {
                    return;
                }

                _currentLongExposed = value;
                RaisePropertyChanged(CurrentLongExposedPropertyName);
            }
        }
        public string CurrentTotal
        {
            get
            {
                return _currentTotal;
            }

            set
            {
                if (_currentTotal == value)
                {
                    return;
                }

                _currentTotal = value;
                RaisePropertyChanged(CurrentTotalPropertyName);
            }
        }
        #endregion CURRENT-VALUE PROPERTIES

        #region FPS-VALUE PROPERTIES
        public const string FPSBodyIndexPropertyName = "FPSBodyIndex";
        public const string FPSBodyPropertyName = "FPSBody";
        public const string FPSColorPropertyName = "FPSColor";
        public const string FPSDepthPropertyName = "FPSDepth";
        public const string FPSInfraredPropertyName = "FPSInfrared";
        public const string FPSLongExposedPropertyName = "FPSLongExposed";
        public const string FPSTotalPropertyName = "FPSTotal";
        private string _fpsBody = "-";
        private string _fpsBodyIndex = "-";
        private string _fpsColor = "-";
        private string _fpsDepth = "-";
        private string _fpsInfrared = "-";
        private string _fpsLongExposed = "-";
        private string _fpsTotal = "-";

        public string FPSBody
        {
            get
            {
                return _fpsBody;
            }

            set
            {
                if (_fpsBody == value)
                {
                    return;
                }

                _fpsBody = value;
                RaisePropertyChanged(FPSBodyPropertyName);
            }
        }

        public string FPSBodyIndex
        {
            get
            {
                return _fpsBodyIndex;
            }

            set
            {
                if (_fpsBodyIndex == value)
                {
                    return;
                }

                _fpsBodyIndex = value;
                RaisePropertyChanged(FPSBodyIndexPropertyName);
            }
        }

        public string FPSColor
        {
            get
            {
                return _fpsColor;
            }

            set
            {
                if (_fpsColor == value)
                {
                    return;
                }

                _fpsColor = value;
                RaisePropertyChanged(FPSColorPropertyName);
            }
        }
        public string FPSDepth
        {
            get
            {
                return _fpsDepth;
            }

            set
            {
                if (_fpsDepth == value)
                {
                    return;
                }

                _fpsDepth = value;
                RaisePropertyChanged(FPSDepthPropertyName);
            }
        }
        public string FPSInfrared
        {
            get
            {
                return _fpsInfrared;
            }

            set
            {
                if (_fpsInfrared == value)
                {
                    return;
                }

                _fpsInfrared = value;
                RaisePropertyChanged(FPSInfraredPropertyName);
            }
        }
        public string FPSLongExposed
        {
            get
            {
                return _fpsLongExposed;
            }

            set
            {
                if (_fpsLongExposed == value)
                {
                    return;
                }

                _fpsLongExposed = value;
                RaisePropertyChanged(FPSLongExposedPropertyName);
            }
        }
        public string FPSTotal
        {
            get
            {
                return _fpsTotal;
            }

            set
            {
                if (_fpsTotal == value)
                {
                    return;
                }

                _fpsTotal = value;
                RaisePropertyChanged(FPSTotalPropertyName);
            }
        }
        #endregion FPS-VALUE PROPERTIES

        #region Commands
        private RelayCommand _startMonitorCommand;
        public RelayCommand StartMonitorCommand
        {
            get
            {
                return _startMonitorCommand ?? (_startMonitorCommand = new RelayCommand(StartMonitor));
            }
        }

        private RelayCommand _stopMonitorCommand;
        public RelayCommand StopMonitorCommand
        {
            get
            {
                return _stopMonitorCommand ?? (_stopMonitorCommand = new RelayCommand(StopMonitor));
            }
        }
        #endregion Commands

        public const string IsMonitorRunningPropertyName = "IsMonitorRunning";
        private bool _monitorRunning = false;
        public bool IsMonitorRunning
        {
            get
            {
                return _monitorRunning;
            }

            set
            {
                if (_monitorRunning == value)
                {
                    return;
                }

                _monitorRunning = value;
                RaisePropertyChanged(IsMonitorRunningPropertyName);
            }
        }

    }
}