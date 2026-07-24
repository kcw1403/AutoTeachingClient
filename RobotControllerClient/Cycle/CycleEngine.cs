using System;
using System.Collections.Generic;
using System.Threading;
using RobotControllerClient.Communication;

namespace RobotControllerClient.Cycle
{
    public class CycleProgressEventArgs : EventArgs
    {
        public int Iteration { get; set; }
        public int TotalIterations { get; set; }
        public int StepIndex { get; set; }
        public CycleStep Step { get; set; }
        public CommandResult Result { get; set; }
    }

    public class CycleEngine
    {
        private readonly RobotClient _client;
        private Thread _thread;
        private CancellationTokenSource _cts;

        public bool IsRunning
        {
            get { return _thread != null && _thread.IsAlive; }
        }

        public event EventHandler<CycleProgressEventArgs> StepStarted;
        public event EventHandler<CycleProgressEventArgs> StepFinished;
        public event EventHandler<int> IterationCompleted;
        public event EventHandler<string> CycleStopped;

        public CycleEngine(RobotClient client)
        {
            _client = client;
        }

        public void Start(IList<CycleStep> steps, int iterations, bool infinite, bool stopOnError)
        {
            Start(steps, iterations, infinite, stopOnError, 0);
        }

        public void Start(IList<CycleStep> steps, int iterations, bool infinite, bool stopOnError, int startIndex)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("사이클이 이미 실행 중입니다.");
            }
            if (steps == null || steps.Count == 0)
            {
                throw new InvalidOperationException("실행할 동작이 없습니다.");
            }

            List<CycleStep> snapshot = new List<CycleStep>();
            foreach (CycleStep s in steps)
            {
                snapshot.Add(s.Clone());
            }

            int firstStart = startIndex;
            if (firstStart < 0 || firstStart >= snapshot.Count)
            {
                firstStart = 0;
            }

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            _thread = new Thread(() => Run(snapshot, iterations, infinite, stopOnError, firstStart, token))
            {
                IsBackground = true,
                Name = "CycleEngine"
            };
            _thread.Start();
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }

        private void Run(List<CycleStep> steps, int iterations, bool infinite, bool stopOnError, int firstStart, CancellationToken token)
        {
            string stopReason = "사이클 정상 종료";
            try
            {
                int iteration = 0;
                while (infinite || iteration < iterations)
                {
                    if (token.IsCancellationRequested)
                    {
                        stopReason = "사용자에 의해 중지됨";
                        break;
                    }

                    iteration++;
                    int total = infinite ? 0 : iterations;

                    int start = iteration == 1 ? firstStart : 0;
                    for (int i = start; i < steps.Count; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            stopReason = "사용자에 의해 중지됨";
                            RaiseStopped(stopReason);
                            return;
                        }

                        CycleStep step = steps[i];

                        RaiseStepStarted(iteration, total, i, step);

                        if (step.IsDelay)
                        {
                            if (step.DelayAfterMs > 0 && token.WaitHandle.WaitOne(step.DelayAfterMs))
                            {
                                stopReason = "사용자에 의해 중지됨";
                                RaiseStopped(stopReason);
                                return;
                            }
                            continue;
                        }

                        CommandResult result = _client.SendCommand(step.CommandText, token);

                        RaiseStepFinished(iteration, total, i, step, result);

                        if (!result.IsSuccess && stopOnError)
                        {
                            stopReason = string.Format("에러로 중지 (반복 {0}, 스텝 {1}: {2})",
                                iteration, i + 1, DescribeFailure(result));
                            RaiseStopped(stopReason);
                            return;
                        }

                        if (step.DelayAfterMs > 0)
                        {
                            if (token.WaitHandle.WaitOne(step.DelayAfterMs))
                            {
                                stopReason = "사용자에 의해 중지됨";
                                RaiseStopped(stopReason);
                                return;
                            }
                        }
                    }

                    RaiseIterationCompleted(iteration);
                }
            }
            catch (Exception ex)
            {
                stopReason = "사이클 예외: " + ex.Message;
            }

            RaiseStopped(stopReason);
        }

        private static string DescribeFailure(CommandResult result)
        {
            switch (result.Status)
            {
                case ResultStatus.Nak:
                    return "_NAK (잘못된 명령)";
                case ResultStatus.Error:
                    return string.Format("_ERR {0} {1}", result.ErrorCode, result.ErrorDescription);
                case ResultStatus.Timeout:
                    return "응답 타임아웃";
                case ResultStatus.NotConnected:
                    return "연결 끊김";
                default:
                    return "실패";
            }
        }

        private void RaiseStepStarted(int iteration, int total, int stepIndex, CycleStep step)
        {
            EventHandler<CycleProgressEventArgs> h = StepStarted;
            if (h != null)
            {
                h(this, new CycleProgressEventArgs
                {
                    Iteration = iteration,
                    TotalIterations = total,
                    StepIndex = stepIndex,
                    Step = step
                });
            }
        }

        private void RaiseStepFinished(int iteration, int total, int stepIndex, CycleStep step, CommandResult result)
        {
            EventHandler<CycleProgressEventArgs> h = StepFinished;
            if (h != null)
            {
                h(this, new CycleProgressEventArgs
                {
                    Iteration = iteration,
                    TotalIterations = total,
                    StepIndex = stepIndex,
                    Step = step,
                    Result = result
                });
            }
        }

        private void RaiseIterationCompleted(int iteration)
        {
            EventHandler<int> h = IterationCompleted;
            if (h != null)
            {
                h(this, iteration);
            }
        }

        private void RaiseStopped(string reason)
        {
            EventHandler<string> h = CycleStopped;
            if (h != null)
            {
                h(this, reason);
            }
        }
    }
}
