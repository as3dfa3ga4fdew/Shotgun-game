const MIN_USERNAME_LENGTH = 5;
const MAX_USERNAME_LENGTH = 16;

const ALPHABET = "abcdefghijklmnopqrstuvwxyz";
const ALPHABET_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
const NUMBERS = "0123456789";
const SPECIAL = "~!@#$%^&*()-_+={}][|\\`,./?;:'\"<>";

const IsValidUsername = (username) =>{
    
    if (username === null) return false;
    if (username === undefined) return false;
   
    const length = username.length;

    if (length < MIN_USERNAME_LENGTH || length > MAX_USERNAME_LENGTH) return false;

    const validChars = ALPHABET + ALPHABET_UPPER + NUMBERS;
    const containsValidChars = [...username].every(char => {
        const isValid = validChars.includes(char);
        return isValid;
    });

    return containsValidChars;
}

const IsValidPassword = (password) =>{
    if (password === null) return false;
    if (password === undefined) return false;

    const length = password.length;

    if (length < 8 || length > 50) return false;

    const containsAllTypes =
    [ALPHABET, ALPHABET_UPPER, NUMBERS, SPECIAL].every(type => [...password].some(char => type.includes(char)));

    const containsEachType =
        [ALPHABET, ALPHABET_UPPER, NUMBERS, SPECIAL].every(type => [...password].some(char => type.includes(char)));

    return containsAllTypes && containsEachType;
}

export { IsValidUsername, IsValidPassword };