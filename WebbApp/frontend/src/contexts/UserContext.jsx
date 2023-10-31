import { createContext, useContext, useState, useEffect } from "react";

const UserContext = createContext();

const UserProvider = (props) =>{
    const keyName = "user";
    const userBuilder = {jwt: "", username: "", type: 0, rememberMe: false};

    const userUpdate = (val) =>{
        setUser(val);
    }

    const [user, setUser] = useState(null);

    //get user object from local storage and call api/auth/validate to check if the jwt token is still valid
    useEffect(() => {
        let value = JSON.parse(sessionStorage.getItem(keyName));

        //If session storage has user then no need to validate jwt
        if(value)
        {
            setUser(value);
            return;
        }

        value = JSON.parse(localStorage.getItem(keyName));

        //if local storage has no user then set new user obj and return
        if(!value) {
            setUser(userBuilder);
            return;
        }

        SetValidatedUserAsync(value.jwt, setUser, value, userBuilder);

    }, []);

    useEffect(() => {
        if(user === null)
            return;
        
        //Always set user to session storage
        sessionStorage.setItem(keyName, JSON.stringify(user));

        //Do not save the user object to local storage if remember me is false
        if(user.rememberMe === false)
        {
            localStorage.removeItem(keyName);
            return;
        }

        localStorage.setItem(keyName, JSON.stringify(user));
    }, [user]);

    return(
        <UserContext.Provider value={{user, userUpdate}}>
            {props.children}
        </UserContext.Provider>
    );
}

//Validates the jwt token stored in session/local storage and sets it in the usercontext
const SetValidatedUserAsync = async (jwt, setUser, value, userBuilder) =>{
    try
    {
        let response = await fetch("https://localhost:5000/api/user/token", {
            headers: {
                Authorization: "Bearer " + jwt
            }
        });

        //if response code is not ok then set new user obj and return
        if(response.ok !== true)
        {
            setUser(userBuilder);
            return;
        }
    }
    catch(error)
    {
        setUser(userBuilder);
        console.log(error);
    }

    setUser(value);
    return;
}

export { UserProvider, UserContext };