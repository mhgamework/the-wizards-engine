using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
	public class Invoker
	{

		public Invoker()
		{
			Invokes = new List<IAction>();
		}

		public void ProcessInvokes(int nTime)
		{
			//TODO: is dit wel snel genoeg?

			List<IAction> Acts = new List<IAction>(16);
			IAction IA;

			lock (invokesLock)
			{
				for (int i = 0; i < Invokes.Count; i++)
				{
					IA = Invokes[i];
					if (IA.Time < nTime) Acts.Add(IA);



				}
				for (int i = 0; i < Acts.Count; i++)
				{
					Invokes.Remove(Acts[i]);


				}
			}

			for (int i = 0; i < Acts.Count; i++)
			{
				Acts[i].Invoke();
			}


		}

		//////////Public Sub ProcessScheduledActions(ByVal sender As Object, ByVal e As ProcessElement.ProcessEventArgs)
		//////////    Dim Actions As Action() = Nothing
		//////////    Dim noActs As Boolean = False

		//////////    'dit werkt niet, op een of andere manier
		//////////    'SyncLock Me.ScheduledActions
		//////////    SyncLock Me
		//////////        If Me.ScheduledActions.Count = 0 Then
		//////////            noActs = True
		//////////        Else
		//////////            Actions = Me.ScheduledActions.ToArray
		//////////        End If

		//////////    End SyncLock
		//////////    If noActs Then Exit Sub


		//////////    Dim RemovedActions As New List(Of Action)

		//////////    For Each A As Action In Actions
		//////////        If A.Expired(Me.HoofdObj.Time) Then
		//////////            A.Invoke()
		//////////            RemovedActions.Add(A)
		//////////        End If
		//////////    Next

		//////////    'SyncLock Me.ScheduledActions
		//////////    SyncLock Me
		//////////        For Each a As Action In RemovedActions
		//////////            Me.ScheduledActions.Remove(a)
		//////////        Next
		//////////    End SyncLock
		//////////End Sub



		/// <summary>
		/// Thread safe.
		/// </summary>

		public void Invoke(Action_VoidCallback nCallback)
		{
			lock (invokesLock)
			{
				Invokes.Add(new Action_Void(nCallback, 0));

			}
		}
		public void Invoke<T>(Action_EventArgsCallback<T> nCallback, object sender, T e) where T : EventArgs
		{
			lock (invokesLock)
			{
				Invokes.Add(new Action_EventArgs<T>(nCallback, sender, e, 0));

			}
		}
		public void Invoke(Action_VoidCallback nCallback, int nTime)
		{
			lock (invokesLock)
			{
				Invokes.Add(new Action_Void(nCallback, nTime));

			}
		}
		public void Invoke<T>(Action_EventArgsCallback<T> nCallback, object sender, T e, int nTime) where T : EventArgs
		{
			lock (invokesLock)
			{
				Invokes.Add(new Action_EventArgs<T>(nCallback, sender, e, nTime));

			}
		}
		//////////    Public Sub ScheduleAction(ByVal callback As DelegateActionCallback, ByVal nTimeLeft As Integer)
		//////////    'If System.Threading.Monitor.TryEnter(Me.ScheduledActions, 10000) = False Then
		//////////    '    Throw New Exception("Unable to lock Me.ScheduledActions!")
		//////////    'End If
		//////////    SyncLock Me '.ScheduledActions
		//////////        If Me.HoofdObj Is Nothing Then Exit Sub
		//////////        Me.ScheduledActions.Add(New DelegateAction(callback, Me.HoofdObj.Time + nTimeLeft))
		//////////    End SyncLock
		//////////    'System.Threading.Monitor.Exit(Me.ScheduledActions)
		//////////End Sub

		//////////''' <summary>
		//////////''' Thread safe.
		//////////''' </summary>
		//////////''' <param name="callback"></param>
		//////////''' <param name="nTimeLeft"></param>
		//////////''' <remarks></remarks>
		//////////Public Sub ScheduleAction(Of t)(ByVal callback As DelegateActionCallback(Of t), ByVal Args As t, ByVal nTimeLeft As Integer)
		//////////    'If System.Threading.Monitor.TryEnter(Me.ScheduledActions, 10000) = False Then
		//////////    '    Throw New Exception("Unable to lock Me.ScheduledActions!")
		//////////    'End If
		//////////    SyncLock Me '.ScheduledActions
		//////////        If Me.HoofdObj Is Nothing Then Exit Sub
		//////////        Me.ScheduledActions.Add(New DelegateAction(Of t)(callback, Args, Me.HoofdObj.Time + nTimeLeft))
		//////////    End SyncLock
		//////////    'System.Threading.Monitor.Exit(Me.ScheduledActions)
		//////////End Sub


		//////////Public Sub ScheduleAction(Of t As EventArgs)(ByVal callback As DelegateActionEventCallback(Of t), ByVal sender As Object, ByVal e As t, ByVal nTimeLeft As Integer)
		//////////    'If System.Threading.Monitor.TryEnter(Me.ScheduledActions, 10000) = False Then
		//////////    '    Throw New Exception("Unable to lock Me.ScheduledActions!")
		//////////    'End If
		//////////    SyncLock Me '.ScheduledActions
		//////////        If Me.HoofdObj Is Nothing Then Exit Sub
		//////////        Me.ScheduledActions.Add(New DelegateActionEvent(Of t)(callback, sender, e, Me.HoofdObj.Time + nTimeLeft))
		//////////    End SyncLock
		//////////    'System.Threading.Monitor.Exit(Me.ScheduledActions)
		//////////End Sub


		private object invokesLock = new object();
		private List<IAction> _invokes;

		public List<IAction> Invokes
		{
			get { return _invokes; }
			set { _invokes = value; }
		}




		public interface IAction
		{
			int Time
			{
				get;
			}

			void Invoke();
		}

		public delegate void Action_VoidCallback();
		public delegate void Action_EventArgsCallback<T>(object sender, T e) where T : EventArgs;

		public class Action_Void : IAction
		{
			public Action_Void(Action_VoidCallback callback, int time)
			{
				Callback = callback;
				Time = time;
			}

			private Action_VoidCallback _callback;
			public Action_VoidCallback Callback
			{
				get { return _callback; }
				private set { _callback = value; }
			}

			private int _time;
			public int Time
			{
				get { return _time; }
				set { _time = value; }
			}

			public void Invoke()
			{
				Callback();
			}
		}


		public class Action_EventArgs<T> : IAction where T : EventArgs
		{
			public Action_EventArgs(Action_EventArgsCallback<T> callback, object sender, T e, int time)
			{
				Callback = callback;
				Sender = sender;
				E = e;
				Time = time;
			}

			private Action_EventArgsCallback<T> _callback;
			public Action_EventArgsCallback<T> Callback
			{
				get { return _callback; }
				private set { _callback = value; }
			}

			private int _time;
			public int Time
			{
				get { return _time; }
				set { _time = value; }
			}

			private object _sender;
			public object Sender
			{
				get { return _sender; }
				private set { _sender = value; }
			}

			private T _e;
			public T E
			{
				get { return _e; }
				private set { _e = value; }
			}


			public void Invoke()
			{
				Callback(Sender, E);
			}
		}
	}
}
