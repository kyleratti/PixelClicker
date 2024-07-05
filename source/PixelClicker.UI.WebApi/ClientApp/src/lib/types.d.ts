export type ExternalResourceState<T> =
	| { kind: "pending" }
	| { kind: "error"; error: unknown; }
	| { kind: "done"; value: T; };
