namespace Tuxblox.Model.Entities
{
    public class NodeStatusEntity : BaseEntity
    {
        private string _Status;
        private int _BlockHeight;
        private int _Headers;
        private int _Connections;

        /// <summary>
        /// Gets and sets the Status property.
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set { SetValue(ref _Status, value); }
        }

        /// <summary>
        /// Gets and sets the BlockHeight property.
        /// </summary>
        public int BlockHeight
        {
            get { return _BlockHeight; }
            set { SetValue(ref _BlockHeight, value); }
        }

        /// <summary>
        /// Gets and sets the Headers property.
        /// </summary>
        public int Headers
        {
            get { return _Headers; }
            set { SetValue(ref _Headers, value); }
        }

        /// <summary>
        /// Gets and sets the Connections property.
        /// </summary>
        public int Connections
        {
            get { return _Connections; }
            set { SetValue(ref _Connections, value); }
        }
    }
}
