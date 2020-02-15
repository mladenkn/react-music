import { MusicDbTrackQueryParamas } from "../shared/trackQueryForm";
import { useFormik } from "formik";
import { useEffect, useState } from "react";
import { difference } from 'lodash'

type Field = keyof MusicDbTrackQueryParamas

interface MusicDbTrackQueryFormLogic {
	values: MusicDbTrackQueryParamas
	inactiveFields: Field[]
	availableTags: string[]
	isFieldActive(fieldName: Field): boolean
	setFieldActive(fieldName: Field): void
	setFieldInactive(fieldName: Field): void
	onFieldChange(fieldName: Field): (value: any) => void
}

interface MusicDbTrackQueryFormLogicProps {
	values: MusicDbTrackQueryParamas
	onChange: (values: MusicDbTrackQueryParamas) => void
}

const allFields: Field[] = ['titleContains', 'youtubeChannelId', 'mustHaveEveryTag', 'mustHaveAnyTag', 'yearRange']

// const getInitialActiveFields = (fields: MusicDbTrackQueryForm) => {
// 	const result: Field[] = []
// 	if(!fields.mustHaveAnyTag && fields.mustHaveAnyTag !== [])
// 		result.push('mustHaveAnyTag')	
// 	if(!fields.mustHaveEveryTag && fields.mustHaveEveryTag !== [])
// 		result.push('mustHaveEveryTag')	
// 	if(!fields.titleContains && fields.titleContains !== '')
// 		result.push('titleContains')
// 	if(!fields.yearRange && fields.yearRange !== '')
// 		result.push('titleContains')
	
// 	return result;
// }

const getFieldDefaultValue = (field: Field) => {
	switch (field) {
    case "youtubeChannelId":
      return undefined;
    case "mustHaveEveryTag":
      return [];
    case "mustHaveAnyTag":
      return [];
    case "titleContains":
      return '';
    case "yearRange":
      return {};
  }
}

export const useMusicDbTrackQueryFormLogic = (props: MusicDbTrackQueryFormLogicProps): MusicDbTrackQueryFormLogic => {
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => { }
	})

	const [activeFields, setActiveFields] = useState<Field[]>([])

	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	const isFieldActive = (fieldName: Field) => {
		return activeFields.includes(fieldName)
	}

	const onFieldChange = (fieldName: Field) => {
		return (value: any) => {
			form.setFieldValue(fieldName, value)
		}
	}

	const setFieldInactive = (fieldName: Field) => {
		onFieldChange(fieldName)(getFieldDefaultValue(fieldName))
		setActiveFields(activeFields.filter(f => f !== fieldName))
	}

	const setFieldActive = (fieldName: Field) => {
		setActiveFields([...activeFields, fieldName])
	}

	const inactiveFields = difference(allFields, activeFields)
	
  const availableTags = ['trance', 'techno', 'house', 'acid']

	console.log({inactiveFields})

	return { values: form.values, availableTags, inactiveFields, isFieldActive, onFieldChange, setFieldInactive, setFieldActive }
}