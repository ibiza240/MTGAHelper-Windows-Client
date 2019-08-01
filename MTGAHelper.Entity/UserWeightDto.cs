using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Lib
{
    public class UserWeightDto
    {
        public float Main { get; set; }
        public float Sideboard { get; set; }

        public UserWeightDto()
        {
        }

        public UserWeightDto(float main, float sideboard)
        {
            Main = main;
            Sideboard = sideboard;
        }
    }
}
