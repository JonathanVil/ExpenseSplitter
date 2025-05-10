<script lang="ts">
    import {getGroups, createGroup} from '$lib/stores/groupsStore';
    import Modal from "$lib/components/Modal.svelte";

    let groups = getGroups();

    let showCreateGroupModal = $state(false);

    function openModal() {
        showCreateGroupModal = true;
    }

    let groupName = $state('');

    async function createNew() {
        await createGroup(groupName);
        showCreateGroupModal = false;
        groupName = '';
    }

</script>

<div class="py-10">
    <header>
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <h1 class="text-3xl font-bold text-gray-900">Groups</h1>
        </div>
    </header>
    <main>
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="bg-white shadow overflow-hidden sm:rounded-lg mt-6">
                <div class="px-4 py-5 sm:p-6">
                    <h3 class="text-xl leading-6 font-medium text-gray-900">
                        Expense Groups
                    </h3>

                    <div class="mt-4 text-sm text-gray-500">
                        These are your groups.
                    </div>

                    {#await groups}
                        <p>...loading</p>
                    {:then groups}
                        <div class="mt-8 grid grid-cols-1 gap-6 sm:grid-cols-2">
                            {#each groups as group}
                                <div class="border border-gray-200 rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow">
                                    <h4 class="text-lg font-medium text-gray-900">{group.name}</h4>
                                    <p>{group.description}</p>

                                    {#if group.isAdmin}
                                        <p class="text-gray-600">You are an admin of this group</p>
                                    {/if}

                                    <div class="mt-4">
                                        <h4 class="text-md font-medium text-gray-900">Members</h4>
                                        <ol>
                                            {#each group.members as member}
                                                <li class="text-gray-600">{member}</li>
                                            {/each}
                                        </ol>
                                    </div>
                                </div>
                            {/each}
                        </div>
                    {:catch error}
                        <p>Error: {error}</p>
                    {/await}

                    <button
                            type="button"
                            class="mt-8 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                            onclick={openModal}>
                        Create new group
                    </button>
                </div>
            </div>
        </div>
    </main>
</div>

<Modal bind:showModal={showCreateGroupModal} onConfirm={createNew} confirmButtonText="Create"
       closeButtonText="Cancel">
    {#snippet header()}
        <div class="text-2xl font-medium">Create new group</div>
    {/snippet}

    <form class="space-y-4 w-full max-w-md">
        <div>
            <label for="name" class="block text-sm font-medium text-gray-700">Name</label>
            <input
                    type="text"
                    id="name"
                    bind:value={groupName}
                    required
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
        </div>
    </form>
</Modal>
