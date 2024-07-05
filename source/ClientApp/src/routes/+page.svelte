<script lang="ts">
	import * as signalR from "@microsoft/signalr";
	import { canvasConfig, fetchFullCanvasConfig, pixelData } from '$lib/stores/pixelStore';
	import PixelCanvas, { type OnPixelSelectedEvent } from '$lib/components/PixelCanvas.svelte';
	import { onMount } from 'svelte';
	import ColorPresetPicker from '$lib/components/ColorPresetPicker.svelte';
	import type { ExternalResourceState } from '$lib/types';
	import { PUBLIC_API_URL as apiUrl } from '$env/static/public';

	const hexValidationRegex = /^#[0-9A-F]{6}$/i;

	const isNotNull = <T>(x: T | null): x is T => x !== null;

	const selectError = (input: ExternalResourceState<unknown>) => {
		if (input.kind === "error") {
			if (input.error instanceof Error) {
				return input.error.message;
			}

			return "Something went wrong.";
		}

		return null;
	};

	let socketState: ExternalResourceState<signalR.HubConnection> = { kind: "pending" };
	let isInitialized = [$canvasConfig, socketState]
		.every(x => x.kind === "done" || x.kind === "error");
	let initializationErrors: string[] = [$canvasConfig, socketState]
		.map(selectError)
		.filter(isNotNull);

	let sendNewPixel: (opts: { x: number; y: number; hexColor: string }) => void;
	let selectedColor: string = "#000000";

	type NewPixelData = {
		x: number;
		y: number;
		hexColor: string;
	};

	onMount(() => {
		fetchFullCanvasConfig();

		const connection = new signalR.HubConnectionBuilder()
			.withUrl(apiUrl + "hub")
			.build();

		connection.on("NewPixel", (data: NewPixelData) => {
			console.log("received new pixel", data);
			$pixelData[data.y][data.x] = data.hexColor;
			pixelData.set($pixelData);
		});

		connection.start()
			.then(() => {
				console.log('connected!');
				socketState = { kind: "done", value: connection };
			})
			.catch(err => {
				console.error('failed to connect', err);
				socketState = { kind: "error", error: err };
			});

		sendNewPixel = (opts: { x: number; y: number; hexColor: string }) => {
			connection.invoke('NewPixel', opts);
		};

		return () => {
			connection.stop();
		};
	});

	const onPixelSelected = (event: CustomEvent<OnPixelSelectedEvent>) => {
		console.log("pixel selected");
		sendNewPixel({
			x: event.detail.x,
			y: event.detail.y,
			hexColor: event.detail.hexColor,
		});
	};
</script>

<h1 class="text-4xl font-bold">
	Pixel Clicker
</h1>

{#if $canvasConfig.kind === "pending" || socketState.kind === "pending"}
	Loading...
{:else if $canvasConfig.kind === "error" || socketState.kind === "error"}
	<div class="text-red-500">
		{#each initializationErrors as error}
			<div>{error}</div>
		{/each}
	</div>
{:else}
	<div>
		<h2 class="text-2xl font-bold">Color</h2>

		<ColorPresetPicker {selectedColor} on:colorSelected={e => selectedColor = e.detail} />
	</div>

	<div>
		<h2 class="text-2xl font-bold">
			Pixel Canvas
		</h2>

		<PixelCanvas width={$canvasConfig.value.width}
								 height={$canvasConfig.value.height}
								 pixelData={$pixelData}
								 {selectedColor}
								 defaultColor="#ffffff"
								 on:pixelSelected={onPixelSelected} />
	</div>
{/if}
