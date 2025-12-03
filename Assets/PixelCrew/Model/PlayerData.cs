using System;

namespace Assets.PixelCrew.Model
{
    [Serializable]
    public class PlayerData
    {
        public int Coins;
        public int Hp;
        public bool IsArmed;

        public void CopyFrom(PlayerData other)
        {
            Coins = other.Coins;
            Hp = other.Hp;
            IsArmed = other.IsArmed;
        }
    }
}
