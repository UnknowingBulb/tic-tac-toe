import React, { Component } from 'react';
import { Modal } from './Modal';
import TriggerButton from './TriggerButton';

export class Container extends Component {
    state = { isShown: false };

    showModal = () => {
        this.setState({ isShown: true }, () => {
            this.closeButton.focus();
        });
    };

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
            <React.Fragment>
                <TriggerButton
                    showModal={this.showModal}
                    buttonRef={(n) => (this.TriggerButton = n)}
                />{this.state.isShown ? (
                    <Modal
                        onSubmit={(event) => { this.props.onSubmit(event); this.closeModal(); }}
                        modalRef={(n) => (this.modal = n)}
                        buttonRef={(n) => (this.closeButton = n)}
                        closeModal={this.closeModal}
                        onKeyDown={this.onKeyDown}
                        onClickOutside={this.onClickOutside}
                    />
                ) : null}
            </React.Fragment>
        );
    }
}
export default Container;