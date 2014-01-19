namespace GameLogic.Models
{
    public class CityModel
    {
        public int HexagonIndex
        {
            get;
            set;
        }

        public int Position
        {
            get;
            set;
        }

        public int PlayerId
        {
            get;
            set;
        }

        public char CitySize
        {
            get;
            set;
        }

        public int HexA { get; set; }
        public int HexB { get; set; }
        public int HexC { get; set; }
    }
}