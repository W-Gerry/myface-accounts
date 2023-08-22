import React, {createContext, ReactNode, useState} from "react";

export const LoginContext = createContext<{
    username?: string,
    password?: string,
    isLoggedIn: boolean,
    isAdmin: boolean,
    logIn: (username?: string, password?: string) => void,
    logOut: () => void,
}>({
    username: undefined,
    password: undefined,
    isLoggedIn: false,
    isAdmin: false,
    logIn: () => {},
    logOut: () => {},
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [username, setUsername] = useState<string>()
    const [password, setPassword] = useState<string>()
    
    async function logIn(username?: string, password?: string) {
        /* Check authentication */
        const response = await fetch(`https://localhost:5001/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({username, password}),
        });
        if (response.ok)
        {
            setLoggedIn(true);
            console.log("Yay");
        }
        else
        {
            setLoggedIn(false);
            console.log("Boo");
        }        
    }
    
    function logOut() {
        setLoggedIn(false);
        setUsername(undefined);
        setPassword(undefined);
    }
    
    const context = {
        username: username,
        password: password,
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        logIn: logIn,
        logOut: logOut,
    };
    
    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}