namespace Model.Map
{
    public class EmptyTile : ITile
    {
        public bool IsOccupied { get; private set; }
        public bool IsReadyToOccupy { get; private set; }

        public void SetOccupied(bool occupied)
        {
            IsOccupied = occupied;
        }

        public void SetReadyToOccupy(bool ready)
        {
            IsReadyToOccupy = ready;
        }
    }
}