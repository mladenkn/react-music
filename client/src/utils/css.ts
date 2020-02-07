export const percent = (v1: number, v2?: number, v3?: number, v4?: number) => {
  let result = `${v1}%`
  if(v2)
    result = result + ` ${v2}%`
  if(v3)
    result = result + ` ${v3}%`
  if(v4)
    result = result + ` ${v4}%`
  result = result + ';'
  return result
}

export const ems = (v1: number, v2?: number, v3?: number, v4?: number) => {
  let result = `${v1}em`
  if(v2)
    result = result + ` ${v2}em`
  if(v3)
    result = result + ` ${v3}em`
  if(v4)
    result = result + ` ${v4}em`
  result = result + ';'
  return result
}