export const ems = (n1: number, n2?: number, n3?: number, n4?: number) =>
    `${n1}em ${(n2 !== undefined) ? n2 + 'em' : ''}  ${(n3 !== undefined) ? n3 + 'em' : ''} ${(n4 !== undefined) ? n4 + 'em' : ''}`

export const rems = (n1: number, n2?: number, n3?: number, n4?: number) => 
    `${n1}rem ${n2 ? n2 + 'rem' : ''} ${n3 ? n3 + 'rem' : ''} ${n4 ? n4 + 'rem' : ''}`

export const percent = (n1: number, n2?: number, n3?: number, n4?: number) => 
    `${n1}% ${n2 ? n2 + '%' : ''} ${n3 ? n3 + '%' : ''} ${n4 ? n4 + '%' : ''}`

export const rgba = (v1: number, v2: number, v3: number, v4: number) => `rgba(${v1}, ${v2}, ${v3}, ${v4})`