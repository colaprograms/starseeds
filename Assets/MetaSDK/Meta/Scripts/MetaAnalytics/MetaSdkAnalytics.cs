// Copyright © 2018, Meta Company.  All rights reserved.
// 
// Redistribution and use of this software (the "Software") in binary form, without modification, is 
// permitted provided that the following conditions are met:
// 
// 1.      Redistributions of the unmodified Software in binary form must reproduce the above 
//         copyright notice, this list of conditions and the following disclaimer in the 
//         documentation and/or other materials provided with the distribution.
// 2.      The name of Meta Company (“Meta”) may not be used to endorse or promote products derived 
//         from this Software without specific prior written permission from Meta.
// 3.      LIMITATION TO META PLATFORM: Use of the Software is limited to use on or in connection 
//         with Meta-branded devices or Meta-branded software development kits.  For example, a bona 
//         fide recipient of the Software may incorporate an unmodified binary version of the 
//         Software into an application limited to use on or in connection with a Meta-branded 
//         device, while he or she may not incorporate an unmodified binary version of the Software 
//         into an application designed or offered for use on a non-Meta-branded device.
// 
// For the sake of clarity, the Software may not be redistributed under any circumstances in source 
// code form, or in the form of modified binary code – and nothing in this License shall be construed 
// to permit such redistribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL META COMPANY BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System.Collections.Generic;
using System.Threading;
using Meta.Mouse;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meta
{
    /// <summary>
    /// Reports analytics events for the SDK.
    /// </summary>
    internal class MetaSdkAnalytics : IEventReceiver
    {
        private int _numberOfSuccessfulSlamInitializations = 0;
        private int _numberOfFailedSlamInitializations = 0;
        private float _slamSuccessfulRelocalizationDuration = float.PositiveInfinity;
        private bool _slamRelocalizationSuccessful = false;
        private bool _slamRelocalizationChanceSpent = false;
        private bool _slamImuStartedOk = false;

        private float _slamInitBeginTime = float.NegativeInfinity;

        /// <summary>
        /// Whether SLAM had begun initializing. This is used to conditionally record 
        /// events when SLAM ends initialization. This is required because SLAM may end
        /// initialization without beginning initialization by loading a map.
        /// </summary>
        private bool _slamBeganInitialization;

        private List<float> _slamInitTimes = new List<float>();

        public void Init(IEventHandlers eventHandlers)
        {
        }


        private void InitSlamLocalizerAnalytics()
        {
            SlamLocalizer slamLocalizer = GameObject.FindObjectOfType<SlamLocalizer>();

            if (slamLocalizer == null)
            {
                Debug.LogError(GetType() + ": Could not retrieve localizer.");
                return;
            }

            slamLocalizer.onSlamSensorsReady.AddListener(() => {  _slamInitBeginTime = Time.fixedUnscaledTime;});
            slamLocalizer.onSlamSensorsReady.AddListener(BeginLocalizationEvent);
            slamLocalizer.onSlamLocalizerResetEvent.AddListener(BeginLocalizationEvent);

            slamLocalizer.onSlamMappingComplete.AddListener(() => { EndLocalizationEvent(true); });
            slamLocalizer.onSlamInitializationFailed.AddListener(() => { EndLocalizationEvent(false); });
        }

        private void BeginLocalizationEvent()
        {
            _slamBeganInitialization = true;
            _slamInitBeginTime = Time.fixedUnscaledTime;
        }

        private void EndLocalizationEvent(bool success)
        {
            SlamLocalizer slamLocalizer = GameObject.FindObjectOfType<SlamLocalizer>();
            if (slamLocalizer)
            {
                RecordSlamSuccessRate(success);
                _slamImuStartedOk = slamLocalizer.IsTracking();

                if (slamLocalizer.SlamInitializedFromLoadedMap && !_slamRelocalizationChanceSpent)
                {
                    _slamSuccessfulRelocalizationDuration = Time.fixedUnscaledTime - _slamInitBeginTime;
                    _slamRelocalizationSuccessful = true;
                }
                else if (_slamBeganInitialization)
                {
                    _slamInitTimes.Add(Time.fixedUnscaledTime - _slamInitBeginTime);
                    _slamBeganInitialization = false;
                }

                _slamRelocalizationChanceSpent = true;
            }
        }

        private void RecordSlamSuccessRate(bool success)
        {
            if (!success)
            {
                _numberOfFailedSlamInitializations++;
            }
            else if (success && _slamBeganInitialization)
            {
                _numberOfSuccessfulSlamInitializations++;
            }
        }


    }
}
