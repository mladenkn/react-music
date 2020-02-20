import React, { useEffect } from 'react'
import { TracklistOptions } from "../shared/homeSectionOptions"
import { InputLabel, Select, MenuItem, Switch, TextField, makeStyles, createStyles, Button } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'
import { ems, percent } from '../../utils/css'

interface HomeSectionOptionsUIProps {
	className?: string
	values: TracklistOptions
	onChange: (f: TracklistOptions) => void
	onSearch: () => void
}

const useStyles = makeStyles(
	() => createStyles({
		root: {
			display: 'flex',
			flexDirection: 'column',
			height: 'fit-content',
		},
		dataSource: {
		},
		fields: {
			marginTop: ems(1),
		},
		searchQueryField: {
			marginTop: ems(0.5),
			maxWidth: percent(83),
		},
		switchInput: {
			display: 'flex',
			alignItems: 'center',
    },
    autoRefreshInput: {
      marginTop: ems(1),
    },
		searchButton: {			
			width: ems(5.5),
			alignSelf: 'flex-end',
		}
	}), {name: 'TrackQueryFormUi'}
)

export const TracklistOptionsUI = (props: HomeSectionOptionsUIProps) => {

	const styles = useStyles()
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => {}
	})
	useEffect(() => {
		props.onChange(form.values)
	}, [form.values])

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				className={styles.dataSource}
				label='Data source'
				value={form.values.queryForm.dataSource}
				onChange={e => form.setFieldValue('queryForm.dataSource', e.target.value)}
			>
				<MenuItem value='MusicDb'>Music DB</MenuItem>
				<MenuItem value='YouTube'>YouTube</MenuItem>
			</Select>

			{form.values.queryForm.dataSource === 'MusicDb' &&
				<MusicDbTrackQueryInteractiveForm
					className={styles.fields}
					input={form.values.queryForm.musicDbParams!}
					onChange={value => form.setFieldValue('queryForm.musicDbParams', value)}
				/>
			} 

			{form.values.queryForm.dataSource === 'YouTube' &&
				<TextField
					className={styles.searchQueryField}
					label='Search Query'
					value={form.values.queryForm.searchQuery!}
					onChange={e => form.setFieldValue('queryForm.searchQuery', e.target.value)}
				/>
			}
			
			<div className={clsx(styles.switchInput, styles.autoRefreshInput)}>
				<InputLabel>Auto refresh:</InputLabel>
				<Switch 
					checked={form.values.autoRefresh} 
					onChange={e => form.setFieldValue('autoRefresh', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.switchInput}>
				<InputLabel>Auto play:</InputLabel>
				<Switch 
					checked={form.values.autoPlay} 
					onChange={e => form.setFieldValue('autoPlay', e.target.checked)}
					color='primary'
				/>
			</div>

			<Button className={styles.searchButton} color="primary" onClick={props.onSearch}>Search</Button>
		</div>
	)
}