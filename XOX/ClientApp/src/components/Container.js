import React, { Component, createRef } from 'react';
import { Form } from './Form';
import TriggerButton from './TriggerButton';

export class Container extends Component {
    state = { isShown: false };
    constructor(props) {
        super(props);
        this.wrapperRef = createRef();
        this.handleClickOutside = this.handleClickOutside.bind(this);
    }

    componentDidMount() {
        document.addEventListener("mousedown", this.handleClickOutside);
    }

    componentWillUnmount() {
        document.removeEventListener("mousedown", this.handleClickOutside);
    }

    showModal = () => {
        this.setState({ isShown: true });
    };

    handleClickOutside(event) {
        if (this.wrapperRef && !this.wrapperRef.current.contains(event.target)) {
            this.setState({ isShown: false });
        }
    }

    closeModal = () => {
        this.setState({ isShown: false });
        this.TriggerButton.focus();

    };

    onKeyDown = (event) => {
        if (event.keyCode === 27) {
            this.closeModal();
        }
    };

    onClickOutside = (event) => {
        if (this.modal && this.modal.contains(event.target)) return;
        this.closeModal();
    };

    render() {
        return (
            <div>
                <TriggerButton
                    showModal={this.showModal}
                    buttonRef={(n) => (this.TriggerButton = n)}
                />
                <div ref={this.wrapperRef} >{this.state.isShown ? (
                    <Form user={this.props.user} onSubmit={(event) => { this.props.onSubmit(event); this.closeModal(); }} />

                ) : null}</div>
            </div>
        );
    }
}
export default Container;