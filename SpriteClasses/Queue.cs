
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace _Sprites;
class Queue
{
    private int FrontPointer;
    private int RearPointer;
    private int Size;
    private readonly int MaxSize;
    private List<Keys> Items;
    public Queue(int Size)
    {
        this.Items = new List<Keys>(Size); //Static Array
        FrontPointer = 0;
        RearPointer = -1;
        this.Size = 0;
        this.MaxSize = Size;
    }

    public void enQueue(Keys Key)
    {
        if (!IsFull())
        { // only take inputs if the list isnt full
            Items.Add(Key);
            RearPointer = (RearPointer++) % MaxSize;  //The modulous used to make the loop
        Size++;
        }

    }
    public Keys deQueue()
    {
        if (!IsEmpty())
        {
            var key = Items[FrontPointer];
            Items.RemoveAt(FrontPointer);
            FrontPointer = (FrontPointer ++) % MaxSize; //The modulous used to make the loop
            Size--;
            return key;
        }
        return Keys.None;

    }
    private bool IsFull()
    {
        return Size == MaxSize;
    }
    private bool IsEmpty()
    {
        return Size == 0;
    }
}