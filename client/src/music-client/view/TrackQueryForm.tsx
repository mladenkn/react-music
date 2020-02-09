import React, { useEffect } from 'react'
import { TrackQueryForm, MusicDbTrackQueryForm } from "../shared"
import { Select, MenuItem, FormControl, Typography, TextField, makeStyles, createStyles } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'

interface TrackQueryFormUiProps {
	className?: string
	form: TrackQueryForm
	onChange: (f: TrackQueryForm) => void
}

const useStyles = makeStyles(
	() => createStyles({
		root: {
			display: 'flex',
			flexDirection: 'column'
		}
	})
)

const testInitialValues: TrackQueryForm = {
	dataSource: 'MusicDb',
	fields: {
		mustHaveAnyTag: ['trance', 'techno'],
		mustHaveEveryTag: ['house', 'acid'],
		titleContains: 'mate i jure',
		youtubeChannelId: undefined,
		yearRange: {
			lowerBound: 1990,
			upperBound: 1998
		}
	},
	searchQuery: 'mate i frane'
}

export const TrackQueryFormUi = (props: TrackQueryFormUiProps) => {

	const styles = useStyles()
	const form = useFormik({
		initialValues: testInitialValues,
		onSubmit: () => { }
	})
	useEffect(() => {
		const { dataSource, fields, searchQuery } = form.values
		const mapped = {
			dataSource: dataSource,
			fields: dataSource === 'MusicDb' ? fields : undefined,
			searchQuery: dataSource === 'MusicDb' ? searchQuery : undefined,
		}
		props.onChange(mapped)
	}, [form.values])

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				label='Data source'
				value={form.values.dataSource}
				onChange={e => form.setFieldValue('dataSource', e.target.value)}
			>
				<MenuItem value='MusicDb'>Music DB</MenuItem>
				<MenuItem value='YouTube'>YouTube</MenuItem>
			</Select>
			{form.values.dataSource === 'MusicDb' &&
				<MusicDbTrackQueryInteractiveForm
					input={form.values.fields!}
					onChange={value => form.setFieldValue('fields', value)}
				/>
			}
			{form.values.dataSource === 'YouTube' &&
				<TextField
					label='Search Query'
					value={form.values.searchQuery!}
					onChange={e => form.setFieldValue('searchQuery', e.target.value)}
				/>
			}
		</div>
	)
}