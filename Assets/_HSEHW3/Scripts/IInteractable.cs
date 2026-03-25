namespace _HSEHW3.Scripts
{
    public interface IInteractable
    {
        string PromptText { get; }
        bool CanInteract { get; }
        void Interact();
    }
}
