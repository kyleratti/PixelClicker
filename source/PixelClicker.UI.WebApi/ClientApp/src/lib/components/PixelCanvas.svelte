<script context="module" lang="ts">
	export type OnPixelSelectedEvent = {
		x: number;
		y: number;
		hexColor: string;
	};
</script>

<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import Pixel from '$lib/components/Pixel.svelte';

	export let width: number;
	export let height: number;
	export let pixelData: string[][];
	export let selectedColor: string;
	export let defaultColor: string;

	const dispatch = createEventDispatcher<{ pixelSelected: OnPixelSelectedEvent}>();

	const onPixelSelected = (opts: { x: number; y: number; hexColor: string; }) => {
		const eventProps: OnPixelSelectedEvent = {
			x: opts.x,
			y: opts.y,
			hexColor: opts.hexColor,
		};

		dispatch("pixelSelected", eventProps);
	};

	$: tryGetPixelColor = (x: number, y: number) => {
		if (pixelData[y] === undefined) {
			return null;
		}

		const value = pixelData[y][x];

		if (value === undefined) {
			return null;
		}

		return value;
	};
</script>

<table class="table-fixed border-spacing-0 border-separate">
	{#each Array.from({ length: height }) as _, y}
		<tr class="p-0 m-0">
			{#each Array.from({ length: width }) as _, x}
				{@const pixelColor = tryGetPixelColor(x, y)}

				<td class="p-0 w-max h-max leading-[0]">
					<Pixel hexColor={pixelColor ?? defaultColor}
								 showBorder={pixelColor === null || pixelColor === defaultColor}
								 on:selected={() => onPixelSelected({ x, y, hexColor: selectedColor })} />
				</td>
			{/each}
		</tr>
	{/each}
</table>
