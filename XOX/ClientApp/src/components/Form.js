import React from 'react';

export const Form = ({ user, onSubmit }) => {
    return (
        <form onSubmit={ onSubmit }>
            <div className="form-group">
                <label htmlFor="name">Name</label>
                <input className="form-control" id="name" defaultValue={user.Name} />
            </div>

            <div className="form-group">
                <label htmlFor="mark">Mark</label>
                <input className="form-control" id="mark" defaultValue={user.Mark} />
            </div>

            <div className="form-group">
                <button className="form-control btn btn-primary" type="submit">
                    Submit
                </button>
            </div>
        </form>
    );
};

export default Form;