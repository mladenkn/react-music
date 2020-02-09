import { MusicDbTrackQueryForm } from "../shared";
import { useFormik } from "formik";
import { useEffect } from "react";

type Field = keyof MusicDbTrackQueryForm

interface MusicDbTrackQueryFormLogic {
	isFieldActive(fieldName: Field): boolean
	setFieldActive(fieldName: Field): void
	setFieldInactive(fieldName: Field): void
	values: MusicDbTrackQueryForm
	onFieldChange(fieldName: Field): (value: any) => void
}

interface MusicDbTrackQueryFormLogicProps {
	values: MusicDbTrackQueryForm
	onChange: (values: MusicDbTrackQueryForm) => void
}

export const useMusicDbTrackQueryFormLogic = (props: MusicDbTrackQueryFormLogicProps): MusicDbTrackQueryFormLogic => {
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => { }
	})

	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	const isFieldActive = (fieldName: Field) => {
		return true
	}

	const onFieldChange = (fieldName: Field) => {
		return (value: any) => {
			form.setFieldValue(fieldName, value)
		}
	}

	const setFieldInactive = (fieldName: Field) => {

	}

	const setFieldActive = (fieldName: Field) => {

	}

	return { values: form.values, isFieldActive, onFieldChange, setFieldInactive, setFieldActive }
}