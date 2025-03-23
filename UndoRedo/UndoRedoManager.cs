using System;
using System.Collections.Generic;

namespace MercuryTools.UndoRedo;

public class UndoRedoManager
{
    private Stack<Operation> UndoStack { get; } = new();
    private Stack<Operation> RedoStack { get; } = new();

    public bool CanUndo => UndoStack.Count > 0;
    public bool CanRedo => RedoStack.Count > 0;

    public Operation PeekUndo => UndoStack.Peek();
    public Operation PeekRedo => RedoStack.Peek();

    public event EventHandler? OperationHistoryChanged;
    
    public void Push(Operation operation)
    {
        UndoStack.Push(operation);
        RedoStack.Clear();
        OperationHistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RedoAndPush(Operation operation)
    {
        operation.Redo();
        Push(operation);
    }

    public Operation Undo()
    {
        Operation operation = UndoStack.Pop();
        operation.Undo();
        RedoStack.Push(operation);
        OperationHistoryChanged?.Invoke(this, EventArgs.Empty);

        return operation;
    }

    public Operation Redo()
    {
        Operation operation = RedoStack.Pop();
        operation.Redo();
        UndoStack.Push(operation);
        OperationHistoryChanged?.Invoke(this, EventArgs.Empty);

        return operation;
    }

    public void Clear()
    {
        UndoStack.Clear();
        RedoStack.Clear();
        OperationHistoryChanged?.Invoke(this, EventArgs.Empty);
    }
}