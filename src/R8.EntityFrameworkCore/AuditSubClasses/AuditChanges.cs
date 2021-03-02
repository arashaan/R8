namespace R8.EntityFrameworkCore.AuditSubClasses
{
    public class AuditChanges
    {
        public string Key { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override string ToString()
        {
            return $"[{Key}] FROM [{OldValue}] TO [{NewValue}]";
        }
    }
}