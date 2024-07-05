import { writable } from 'svelte/store';
import type { ExternalResourceState } from '$lib/types';
import { PUBLIC_API_URL as apiUrl } from '$env/static/public';

export type CanvasConfig = {
	width: number;
	height: number;
};

const getData = async () => {
	const resp = await fetch(apiUrl + "canvas");

	if (!resp.ok) {
		throw new Error("Failed to fetch canvas data");
	}

	const data: {
		width: number;
		height: number;
		canvas: string[][];
	} = await resp.json();

	return data;
};

export const canvasConfig = writable<ExternalResourceState<CanvasConfig>>({ kind: "pending" });
export const pixelData = writable<string[][]>([]);

export const fetchFullCanvasConfig = async () => {
	canvasConfig.set({ kind: "pending" });

	try {
		const data = await getData();
		canvasConfig.set({ kind: "done", value: data });
		pixelData.set(data.canvas);
	} catch (err) {
		canvasConfig.set({ kind: "error", error: err });
	}
};
