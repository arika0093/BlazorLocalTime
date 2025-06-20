export function getBrowserTimeZone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}