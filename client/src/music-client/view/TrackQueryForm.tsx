import React, { useEffect } from 'react'
import { TrackQueryForm } from "../shared/trackQueryForm"
import { Select, MenuItem, TextField, makeStyles, createStyles } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'
import { ems, percent } from '../../utils/css'

interface TrackQueryFormUiProps {
	className?: string
	values: TrackQueryForm
	onChange: (f: TrackQueryForm) => void
}

const useStyles = makeStyles(
	() => createStyles({
		root: {
			display: 'flex',
			flexDirection: 'column'
		},
		dataSource: {
			maxWidth: percent(83),
		},
		fields: {
			marginTop: ems(1),
		},
		searchQueryField: {
			marginTop: ems(1)
		},
	}), {name: 'TrackQueryFormUi'}
)

export const TrackQueryFormUi = (props: TrackQueryFormUiProps) => {

	const styles = useStyles()
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => { }
	})
	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				className={styles.dataSource}
				label='Data source'
				value={form.values.dataSource}
				onChange={e => form.setFieldValue('dataSource', e.target.value)}
			>
				<MenuItem value='MusicDb'>Music DB</MenuItem>
				<MenuItem value='YouTube'>YouTube</MenuItem>
			</Select>
			{form.values.dataSource === 'MusicDb' &&
				<MusicDbTrackQueryInteractiveForm
					className={styles.fields}
					input={form.values.musicDbParams!}
					onChange={value => form.setFieldValue('fields', value)}
				/>
			} 
			{form.values.dataSource === 'YouTube' &&
				<TextField
					className={styles.searchQueryField}
					label='Search Query'
					value={form.values.searchQuery!}
					onChange={e => form.setFieldValue('searchQuery', e.target.value)}
				/>
			}
		</div>
	)
}