<script>
    let {
        showModal = $bindable(),
        header,
        children,
        confirmButtonText = 'Confirm',
        closeButtonText = 'Close',
        onConfirm = () => {
        }
    } = $props();

    let dialog = $state(); // HTMLDialogElement

    $effect(() => {
        if (showModal) dialog.showModal();
    });
</script>

<!-- svelte-ignore a11y_click_events_have_key_events, a11y_no_noninteractive_element_interactions -->
<dialog
        bind:this={dialog}
        onclose={() => (showModal = false)}
        onclick={(e) => { if (e.target === dialog) dialog.close(); }}
        class="p-2 max-w-128 rounded-lg"
>
    <div>
        <div class="mb-4 border-b-2 pb-2 border-b-gray-200">
            {@render header?.()}
        </div>
        
        {@render children?.()}
        <!-- svelte-ignore a11y_autofocus -->
        <div class="flex mt-4">
            {#if onConfirm}
                <button onclick={(_) => onConfirm()}
                        class="cursor-pointer px-4 py-2 border-2 border-gray-300  text-base font-medium rounded-md bg-indigo-600 text-white hover:bg-indigo-700">{confirmButtonText}</button>
            {/if}
            <button autofocus onclick={() => dialog.close()}
                    class="cursor-pointer ml-auto px-4 py-2 border-2 border-gray-300  text-base font-medium rounded-md bg-white text-gray-800 hover:bg-gray-50">{closeButtonText}</button>
        </div>
    </div>
</dialog>

<style>
    dialog {
        border-radius: 0.2em;
        border: none;
        position: fixed;
        top: 40%;
        left: 50%;
        transform: translate(-50%, -40%);
    }

    dialog::backdrop {
        background: rgba(0, 0, 0, 0.3);
    }

    dialog > div {
        padding: 1em;
    }

    dialog[open] {
        animation: zoom 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
    }

    dialog[open]::backdrop {
        animation: fade 0.2s ease-out;
    }

    @keyframes fade {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }
</style>
