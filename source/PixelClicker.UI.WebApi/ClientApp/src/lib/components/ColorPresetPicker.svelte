<script lang="ts">
	import Pixel from '$lib/components/Pixel.svelte';
	import { createEventDispatcher } from 'svelte';

	export let selectedColor: string;

	const dispatch = createEventDispatcher<{ colorSelected: string }>();

	const availableColors = [
		"#ffffff",
		"#000000",
		"#ff0000",
		"#00ff00",
		"#0000ff",
		"#ffff00",
		"#ff00ff",
		"#00ffff",
	];
</script>

{#each availableColors as hexColor}
	<div class="mx-1 px-0.5 inline-block border-b-2 hover:border-gray-200 border-transparent"
			 style={`border-color: ${(hexColor === selectedColor ? hexColor : "")}`}>
		<Pixel {hexColor}
					 showBorder={true}
					 on:selected={() => dispatch('colorSelected', hexColor)} />
	</div>
{/each}
