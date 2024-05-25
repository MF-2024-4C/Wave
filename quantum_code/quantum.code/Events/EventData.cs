using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Quantum.Collections;
using Quantum.Core;
using Quantum.Events;

namespace Quantum
{
    public partial struct GameEventInstance
    {
        public GameEventInstance(in MapEvent value, uint id)
        {
            this.value = value;
            this.id = id;
        }
    }

    public partial struct GameEvents
    {
        public unsafe GameEvents(int initialCapacity, FrameBase f)
        {
            var gameEventsData = f.Heap->AllocateAndClear(sizeof(GameEventsData));
            var data = new GameEventsData(initialCapacity, f);
            UnsafeUtility.CopyStructureToPtr(ref data, gameEventsData);
            container = new GameEventsDataPtr()
            {
                ptr = f.Heap->Void(gameEventsData)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameEventWriter GetWriter(FrameBase f)
        {
            return new GameEventWriter(this, f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameEventReader GetReader(FrameBase f)
        {
            return new GameEventReader(this, f);
        }
    }

    public partial struct GameEventsData
    {
        internal QListPtr<GameEventInstance> GetWriteBuffer() => state ? buffer2 : buffer1;
        internal QListPtr<GameEventInstance> GetReadBuffer() => state ? buffer1 : buffer2;

        public GameEventsData(int capacity, FrameBase f)
        {
            var b1 = f.AllocateList<GameEventInstance>(capacity);
            var b2 = f.AllocateList<GameEventInstance>(capacity);
            buffer1 = b1;
            buffer2 = b2;
            eventCounter = 0;
            prevEventCounter = 0;
            state = false;
        }

        public void Update(FrameBase f)
        {
            state = !state;
            if (state) f.ResolveList(buffer2).Clear();
            else f.ResolveList(buffer1).Clear();

            prevEventCounter = eventCounter;
        }

        public void Write(in MapEvent value, FrameBase f)
        {
            if (state) f.ResolveList(buffer2).Add(new GameEventInstance(value, eventCounter));
            else f.ResolveList(buffer1).Add(new GameEventInstance(value, eventCounter));
            eventCounter++;
        }

        public void Free(FrameBase f)
        {
            f.FreeList(buffer1);
            f.FreeList(buffer2);
        }
    }


    public readonly unsafe ref struct GameEventsDataIterator
    {
        public GameEventsDataIterator(GameEventsData* buffer, uint eventCounter, FrameBase f)
        {
            this._buffer = buffer;
            this._eventCounter = eventCounter;
            this._frameBase = f;
        }

        private readonly GameEventsData* _buffer;
        private readonly uint _eventCounter;
        private readonly FrameBase _frameBase;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_frameBase.ResolveList(_buffer->GetReadBuffer()),
                _frameBase.ResolveList(_buffer->GetWriteBuffer()),
                _eventCounter);
        }

        public struct Enumerator : IEnumerator<MapEvent>
        {
            public Enumerator(QList<GameEventInstance> buffer1, QList<GameEventInstance> buffer2,
                uint eventCounter)
            {
                _reader1 = buffer1;
                _reader2 = buffer2;
                _eventCounter = eventCounter;
                _current = default;
                _offset = default;
                _readFirstReader = default;
            }

            private readonly QList<GameEventInstance> _reader1;
            private readonly QList<GameEventInstance> _reader2;
            private readonly uint _eventCounter;
            private MapEvent _current;
            private int _offset;
            private bool _readFirstReader;

            public MapEvent Current => _current;
            object IEnumerator.Current => _current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var reader = _readFirstReader ? _reader2 : _reader1;

                if (((QListPtr<GameEventInstance>)reader).Ptr != Ptr.Null && reader.Count > _offset)
                {
                    ref var instance = ref *reader.GetPointer(_offset);
                    _offset++;

                    if (instance.id < _eventCounter) return MoveNext();
                    _current = instance.value;
                    return true;
                }
                else if (!_readFirstReader)
                {
                    _readFirstReader = true;
                    _offset = 0;
                    return MoveNext();
                }

                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}