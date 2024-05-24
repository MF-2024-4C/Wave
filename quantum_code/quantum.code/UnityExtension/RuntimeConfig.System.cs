using Photon.Deterministic;
using System;

namespace Quantum {
    partial class RuntimeConfig {
        public AssetRefSystemConfig SystemConfig;
        
        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref SystemConfig);
        }
    }
}