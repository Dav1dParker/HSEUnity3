namespace _HSEHW3.Scripts.Interactable
{
    public interface IInteractable
    {
        string PromptText { get; }
        bool CanInteract { get; }
        void Interact();
    }
}
