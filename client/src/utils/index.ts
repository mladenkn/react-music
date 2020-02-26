export const generateArray = <T>(getNext: () => T, count: number) => {
	;
	const r: T[] = [];
	for (let i = 0; i < count; i++)
		r.push(getNext());
	return r;
}

export const replaceMatches = <T>(arr: T[], doesMatch: (item: T) => boolean, replaceWith: T) => {
	const { allItems, updatedItems } = updateMatches(arr, doesMatch, () => replaceWith);
	return { allItems, newItems: updatedItems };
}

export const updateMatches = <T>(arr: T[], doesMatch: (item: T) => boolean, update: (item: T) => T) => {
	const allItems: T[] = [];
	const updatedItems: T[] = [];

	arr.forEach((item) => {
		if (doesMatch(item)) {
			const updated = update(item);
			allItems.push(updated);
			updatedItems.push(updated);
		}
		else
			allItems.push(item);
	});

	return { allItems, updatedItems };
}

export const validURL = (str: string) => {
	var pattern = new RegExp(/^(?:\w+:)?\/\/([^\s\.]+\.\S{2}|localhost[\:?\d]*)\S*$/);
	return !!pattern.test(str);
}

export const containsOnlyDigits = (str: string) => {
	for (let index = 0; index < str.length; index++) {
		const c = str[index];
		if (isNaN(parseInt(c)))
			return false;
	}
	return true;
}

export const capitalize = (str: string) => {
	const firstUpper = str[0].toUpperCase();
	const withoutFirst = str.slice(1, str.length - 1);
	return firstUpper + withoutFirst;
}

export function snapshot(v: unknown){
	return JSON.parse(JSON.stringify(v))
}