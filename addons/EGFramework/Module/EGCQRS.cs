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

    public interface IUndo
    {
        void Undo();
    }

    public interface IUndoCommand: ICommand,IUndo
    {
        
    }
    #endregion

    public class EGCQRS :EGModule
    {
        public void SendCommand(ICommand command)
        {
            command.Execute();
        }

        public void UndoCommand(IUndo command)
        {
            command.Undo();
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

    public static class CanUndoCommandExtension
    {
        public static void EGUndoCommand(this IEGFramework self, IUndo command)
        {
            EGArchitectureImplement.Interface.GetModule<EGCQRS>().UndoCommand(command);
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