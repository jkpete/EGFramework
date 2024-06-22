using System;
using System.Collections.Generic;

namespace EGFramework
{
    #region Interface
    public interface ICommand 
    {
        void Execute();
    }
    public interface IQuery<TResult>
    {
        TResult Do();
    }

    public interface IEGCQRS
    {
        void SendCommand(ICommand command);
        TResult DoQuery<TResult>(IQuery<TResult> query);
    }
    #endregion

    public class EGCQRS :EGModule, IEGCQRS
    {
        public void SendCommand(ICommand command)
        {
            command.Execute();
        }
        public TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            return query.Do();
        }
        public override void Init()
        {
            
        }
    }

    #region Extension
    public static class CanSendCommandExtension
    {
        public static void EGSendCommand(this IEGFramework self, ICommand command)
        {
            EGArchitectureImplement.Interface.GetModule<EGCQRS>().SendCommand(command);
        }
    }

    public static class CanQueryDataExtension
    {
        public static TResult EGQueryData<TResult>(this IEGFramework self, IQuery<TResult> query)
        {
            return EGArchitectureImplement.Interface.GetModule<EGCQRS>().DoQuery(query);
        }
    }
    #endregion
}