<script lang="ts">
	import { createEventDispatcher } from 'svelte';

	export let hexColor: string;
	export let showBorder: boolean;

	const dispatch = createEventDispatcher();

	const onClick = () => {
		dispatch("selected");
	};
</script>

<button type="button"
				class="border border-gray-200 hover:bg-gray-300"
				style={`background-color: ${hexColor}; ${(showBorder ? "" : "border-color: transparent")}`}
				on:click={onClick} />

<style>
	button {
		width: 16px;
		height: 16px;
	}
</style>