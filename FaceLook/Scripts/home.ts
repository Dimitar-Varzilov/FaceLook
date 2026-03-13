export function setSendButtonDisableState(isDisabled: boolean): boolean {
    return (
        (document.getElementById("sendButton") as HTMLButtonElement).disabled =
            isDisabled
    );
}