// messages.ts — ES module

export function closeModal(): void {
    const modal = document.getElementById("deleteModal");
    if (modal) modal.style.display = "none";
}

export function openModal(id: string): void {
    const modal = document.getElementById("deleteModal") as HTMLDivElement | null;
    const input = document.getElementById("modalItemId") as HTMLInputElement | null;
    if (input && modal) {
        input.value = id;
        modal.style.display = "block";
    }
}

// ── event wiring ───────────────────────────────────────────────────────────

window.addEventListener("click", (event: MouseEvent) => {
    const modal = document.getElementById("deleteModal");
    if (event.target === modal) closeModal();
});

document.addEventListener("click", (event: MouseEvent) => {
    const target = (event.target as Element).closest<HTMLElement>(".delete-trigger");
    if (target) {
        const id = target.getAttribute("data-id");
        if (id) openModal(id);
    }
});

// expose helpers on window so inline Razor onclick="" still works
(window as Window & typeof globalThis & Record<string, unknown>)["closeModal"] = closeModal;
(window as Window & typeof globalThis & Record<string, unknown>)["openModal"]  = openModal;
