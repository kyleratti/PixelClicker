<script lang="ts">
	import Pixel from '$lib/components/Pixel.svelte';
	import { createEventDispatcher } from 'svelte';

	export let selectedColor: string;

	const dispatch = createEventDispatcher<{ colorSelected: string }>();

	const availableColors = [
		"#ffffff",
		"#000000",
		"#7c7c7c",
		"#ff0000",
		"#8B0000",
		"#A52A2A",
		"#00ff00",
		"#008000",
		"#006400",
		"#00ffff",
		"#0000ff",
		"#00008B",
		"#ffff00",
		"#FF8C00",
		"#808000",
		"#ff00ff",
		"#800080",
		"#4B0082",
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
